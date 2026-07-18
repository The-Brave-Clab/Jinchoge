using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class GachaTransactionUpdateEntity : BaseEntity<GachaTransactionUpdateEntity>
    {
        public GachaTransactionUpdateEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            RouteConfig config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();

            long gachaId = long.Parse(GetPathParameter("gacha_id"));
            long lineupId = long.Parse(GetPathParameter("lineup_id"));
            long transactionId = long.Parse(GetPathParameter("transaction_id"));

            using var gachasDb = new GachasContext();
            using var cardsDb = new CardsContext();
            using var itemsDb = new ItemsContext();

            if (!player.transactions.gachaTransactions.TryGetValue(transactionId, out var storedTransaction))
                throw new APIErrorException("A1321", $"Unknown gacha transaction {transactionId}.");

            if (gachaId == 0)
                gachaId = storedTransaction.gacha_id;

            if (storedTransaction.gacha_id != gachaId || storedTransaction.lineup_id != lineupId)
                throw new APIErrorException("A1321", $"Gacha transaction {transactionId} does not match gacha {gachaId}/lineup {lineupId}.");

            if (storedTransaction.completed)
            {
                responseBody = Serialize(CreateResponse(transactionId, BuildStoredResults(player, cardsDb, storedTransaction)));
                SetBasicResponseHeaders();
                return Task.CompletedTask;
            }

            GachaLineup? lineup = IsSyntheticFallbackLineup(gachaId, lineupId)
                ? CreateFallbackLineup(gachasDb, gachaId, lineupId)
                : gachasDb.GachaLineups.FirstOrDefault(l => l.Id == lineupId && l.GachaId == gachaId && l.Sp == 1)
                  ?? gachasDb.GachaLineups.FirstOrDefault(l => l.GachaId == gachaId && l.Sp == 1)
                  ?? CreateFallbackLineup(gachasDb, gachaId, lineupId);

            IList<GachaContent> rolledContents = RollCards(gachasDb, cardsDb, lineup, player);
            if (rolledContents.Count > 0 && !DeductConsumption(player, gachasDb, lineup))
                throw new APIErrorException("A1321", "Not enough gacha currency or ticket balance for this lineup.");

            IList<ResultContent> resultContents = new List<ResultContent>();

            foreach (GachaContent rolledContent in rolledContents)
            {
                bool isNew = !player.HasCardForMasterId(rolledContent.ContentId, cardsDb);
                player.GrantCard(rolledContent.ContentId, 1, cardsDb, itemsDb);

                long userCardId = player.GetUserCardIdForMasterId(rolledContent.ContentId, cardsDb);
                resultContents.Add(new ResultContent
                {
                    item_category_id = 1,
                    master_id = rolledContent.ContentId,
                    content_id = rolledContent.ContentId,
                    card = CardsEntity.Card.FromPlayerCardData(cardsDb, userCardId),
                    is_new = isNew
                });
            }

            storedTransaction.completed = true;
            storedTransaction.results = resultContents
                .Select(result => new PlayerProfile.GachaTransactionResult
                {
                    content_id = result.content_id,
                    is_new = result.is_new
                })
                .ToList();
            player.Save();

            responseBody = Serialize(CreateResponse(transactionId, resultContents));
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }


        private static Response CreateResponse(long transactionId, IList<ResultContent> resultContents)
        {
            GachaAnimation animation = CreateAnimation(resultContents);
            return new Response
            {
                free_rare_gacha = 1,
                play_animation = true,
                should_play_animation = true,
                animation = animation,
                gacha_animation = animation,
                transaction = new()
                {
                    id = transactionId,
                    free_rare_gacha = 1,
                    play_animation = true,
                    should_play_animation = true,
                    animation = animation,
                    gacha_animation = animation,
                    results = resultContents
                }
            };
        }

        private static GachaAnimation CreateAnimation(IList<ResultContent> resultContents)
        {
            return new GachaAnimation
            {
                type = "free_rare_gacha",
                name = "free_rare_gacha",
                effect = "free_rare_gacha",
                play = true,
                play_animation = true,
                free_rare_gacha = 1,
                lot_count = Math.Max(resultContents.Count, 1)
            };
        }

        private static IList<ResultContent> BuildStoredResults(
            PlayerProfile player,
            CardsContext cardsDb,
            PlayerProfile.GachaTransaction storedTransaction)
        {
            return storedTransaction.results
                .Select(result =>
                {
                    long userCardId = player.GetUserCardIdForMasterId(result.content_id, cardsDb);
                    return new ResultContent
                    {
                        item_category_id = 1,
                        master_id = result.content_id,
                        content_id = result.content_id,
                        card = CardsEntity.Card.FromPlayerCardData(cardsDb, userCardId),
                        is_new = result.is_new
                    };
                })
                .ToList();
        }

        private static IList<GachaContent> RollCards(
            GachasContext gachasDb,
            CardsContext cardsDb,
            GachaLineup? lineup,
            PlayerProfile player)
        {
            List<GachaContent> cardPool = lineup == null
                ? new List<GachaContent>()
                : gachasDb.GachaContents
                    .Where(c => c.GachaBoxId == lineup.GachaBoxId)
                    .Where(c => c.ContentType == "Card" || c.Category == 1)
                    .Where(c => c.Weight > 0)
                    .ToList();

            cardPool = ApplySelectGachaFilter(gachasDb, lineup, player, cardPool, out bool selectionLimitedPool);

            if (selectionLimitedPool && cardPool.Count == 0)
                return new List<GachaContent>();

            if (cardPool.Count == 0)
            {
                long fallbackGachaBoxId = lineup == null ? 0 : lineup.GachaBoxId;
                cardPool = cardsDb.Cards
                    .Select(c => new GachaContent
                    {
                        GachaBoxId = fallbackGachaBoxId,
                        Category = 1,
                        ContentId = c.Id,
                        ContentType = "Card",
                        Weight = 1
                    })
                    .ToList();
            }

            if (cardPool.Count == 0)
                return new List<GachaContent>();

            int lotCount = Math.Max(lineup?.LotCount ?? 1, 1);
            List<GachaContent> results = new();
            for (int i = 0; i < lotCount; i++)
                results.Add(RollOne(cardPool));

            return results;
        }


        private static List<GachaContent> ApplySelectGachaFilter(
            GachasContext gachasDb,
            GachaLineup? lineup,
            PlayerProfile player,
            List<GachaContent> cardPool,
            out bool selectionLimitedPool)
        {
            selectionLimitedPool = false;

            if (lineup == null || cardPool.Count == 0)
                return cardPool;

            IList<int>? selectedIds = GetPlayerSelection(player, lineup);
            if (selectedIds == null || selectedIds.Count == 0)
                return cardPool;

            selectionLimitedPool = true;

            HashSet<int> selectedSelectIds = selectedIds.ToHashSet();
            HashSet<long> selectedContentIds = gachasDb.GachaContents
                .Where(c => c.GachaBoxId == lineup.GachaBoxId)
                .Where(c => c.SelectId != null && selectedSelectIds.Contains(c.SelectId.Value))
                .Select(c => c.ContentId)
                .ToHashSet();

            if (selectedContentIds.Count == 0)
            {
                HashSet<long> storedContentIds = selectedIds.Select(id => (long)id).ToHashSet();
                selectedContentIds = cardPool
                    .Where(c => storedContentIds.Contains(c.ContentId))
                    .Select(c => c.ContentId)
                    .ToHashSet();
            }

            if (selectedContentIds.Count == 0)
            {
                Utils.LogWarning($"Select gacha selection for gacha {lineup.GachaId}/box {lineup.GachaBoxId} did not match any contents; returning no results instead of rolling unselected cards.");
                return new List<GachaContent>();
            }

            return cardPool
                .Where(c => selectedContentIds.Contains(c.ContentId))
                .ToList();
        }

        private static IList<int>? GetPlayerSelection(PlayerProfile player, GachaLineup lineup)
        {
            long[] selectionKeys =
            {
                lineup.GachaBoxId,
                lineup.GachaId,
                lineup.Id
            };

            foreach (long key in selectionKeys)
            {
                if (player.gachaSelections.TryGetValue(key, out IList<int>? selected) && selected.Count > 0)
                    return selected;
            }

            return null;
        }

        private static bool DeductConsumption(PlayerProfile player, GachasContext gachasDb, GachaLineup? lineup)
        {
            if (lineup == null || lineup.ConsumptionAmount <= 0 || Config.Get().InGame.InfiniteItems)
                return true;

            int amount = lineup.ConsumptionAmount;
            bool deducted = lineup.ConsumptionResourceId switch
            {
                1 => DeductBillingPoint(player, amount),
                2 => DeductFreeBlessing(player, amount),
                3 => DeductFriendPoint(player, amount),
                4 => DeductGameCoinPoint(player, amount),
                6 or 7 => DeductGachaTicket(player, gachasDb, lineup, amount),
                8 => DeductBraveCoin(player, amount),
                21 => DeductExchangePoint(player, amount),
                2001 => true,
                _ => false
            };

            if (!deducted)
            {
                Utils.LogWarning($"Gacha lineup {lineup.Id} could not deduct resource {lineup.ConsumptionResourceId} amount {amount}.");
                return false;
            }

            player.Save();
            return true;
        }

        private static bool DeductBillingPoint(PlayerProfile player, int amount)
        {
            if (player.data.paidBlessing + player.data.freeBlessing < amount)
                return false;

            int paidDeduction = Math.Min(player.data.paidBlessing, amount);
            player.data.paidBlessing -= paidDeduction;

            int remaining = amount - paidDeduction;
            if (remaining > 0)
                player.data.freeBlessing -= remaining;

            return true;
        }

        private static bool DeductFreeBlessing(PlayerProfile player, int amount)
        {
            if (player.data.freeBlessing < amount)
                return false;

            player.data.freeBlessing -= amount;
            return true;
        }

        private static bool DeductFriendPoint(PlayerProfile player, int amount)
        {
            if (player.data.friendPoint < amount)
                return false;

            player.data.friendPoint -= amount;
            return true;
        }

        private static bool DeductGameCoinPoint(PlayerProfile player, int amount)
        {
            if (player.data.money < amount)
                return false;

            player.data.money -= amount;
            return true;
        }

        private static bool DeductBraveCoin(PlayerProfile player, int amount)
        {
            if (player.data.braveCoin < amount)
                return false;

            player.data.braveCoin -= amount;
            return true;
        }

        private static bool DeductExchangePoint(PlayerProfile player, int amount)
        {
            if (player.data.exchangePoint < amount)
                return false;

            player.data.exchangePoint -= amount;
            return true;
        }

        private static bool DeductGachaTicket(
            PlayerProfile player,
            GachasContext gachasDb,
            GachaLineup lineup,
            int amount)
        {
            GachaTicket? ticket = gachasDb.GachaTickets
                .FirstOrDefault(t => t.GachaId == lineup.GachaId && t.ConsumptionResourceId == lineup.ConsumptionResourceId)
                ?? gachasDb.GachaTickets.FirstOrDefault(t => t.ConsumptionResourceId == lineup.ConsumptionResourceId);

            if (ticket == null || !player.items.gachaTickets.TryGetValue(ticket.Id, out long ticketItemId) || !Item.Exists(ticketItemId))
            {
                Utils.LogWarning($"No persisted gacha ticket was found for lineup {lineup.Id}; ticket balance was not deducted.");
                return false;
            }

            Item ticketItem = Item.Load(ticketItemId);
            if (ticketItem.quantity < amount)
                return false;

            ticketItem.quantity -= amount;
            ticketItem.Save();
            return true;
        }

        private static GachaLineup CreateFallbackLineup(GachasContext gachasDb, long gachaId, long lineupId)
        {
            return new GachaLineup
            {
                Id = lineupId,
                GachaId = gachaId,
                GachaBoxId = gachaId,
                LotCount = IsTenRollLineup(lineupId) ? 10 : 1,
                ConsumptionResourceId = GetFallbackConsumptionResourceId(gachasDb, gachaId),
                ConsumptionAmount = IsTenRollLineup(lineupId) ? 2500 : 250,
                Sp = 1,
                FreeRareGacha = 1
            };
        }

        private static int GetFallbackConsumptionResourceId(GachasContext gachasDb, long gachaId)
        {
            Gacha? gacha = gachasDb.Gachas.FirstOrDefault(g => g.Id == gachaId);
            return gacha?.Kind == 1 || gachaId == 1 ? 3 : 1;
        }

        private static bool IsSyntheticFallbackLineup(long gachaId, long lineupId)
        {
            return lineupId == gachaId * 100 + 1 || lineupId == gachaId * 100 + 10;
        }

        private static bool IsTenRollLineup(long lineupId)
        {
            string idText = lineupId.ToString();
            return idText.EndsWith("10", StringComparison.Ordinal);
        }

        private static GachaContent RollOne(IList<GachaContent> pool)
        {
            int totalWeight = pool.Sum(c => c.Weight);
            int roll = Random.Shared.Next(totalWeight);

            foreach (GachaContent content in pool)
            {
                if (roll < content.Weight)
                    return content;

                roll -= content.Weight;
            }

            return pool[^1];
        }

        public class Response
        {
            public Transaction transaction { get; set; } = new();
            public int free_rare_gacha { get; set; }
            public bool play_animation { get; set; }
            public bool should_play_animation { get; set; }
            public GachaAnimation animation { get; set; } = new();
            public GachaAnimation gacha_animation { get; set; } = new();

            public class Transaction
            {
                public long id { get; set; }
                public int free_rare_gacha { get; set; }
                public bool play_animation { get; set; }
                public bool should_play_animation { get; set; }
                public GachaAnimation animation { get; set; } = new();
                public GachaAnimation gacha_animation { get; set; } = new();
                public IList<ResultContent> results { get; set; } = new List<ResultContent>();
            }
        }

        public class GachaAnimation
        {
            public string type { get; set; } = "free_rare_gacha";
            public string name { get; set; } = "free_rare_gacha";
            public string effect { get; set; } = "free_rare_gacha";
            public bool play { get; set; } = true;
            public bool play_animation { get; set; } = true;
            public int free_rare_gacha { get; set; } = 1;
            public int lot_count { get; set; } = 1;
        }

        public class ResultContent
        {
            public int item_category_id { get; set; }
            public long master_id { get; set; }
            public long content_id { get; set; }
            public CardsEntity.Card card { get; set; } = new();
            public bool is_new { get; set; }
        }
    }
}
