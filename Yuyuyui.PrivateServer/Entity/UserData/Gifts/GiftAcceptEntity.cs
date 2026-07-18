using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class GiftAcceptEntity : BaseEntity<GiftAcceptEntity>
    {
        public GiftAcceptEntity(
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
            var acceptedGiftIds = GetTargetGiftIds(player).ToList();

            using var cardsDb = new CardsContext();
            using var itemsDb = new ItemsContext();

            foreach (long giftId in acceptedGiftIds)
            {
                if (!Gift.Exists(giftId))
                {
                    player.receivedGifts.Remove(giftId);
                    continue;
                }

                Gift gift = Gift.Load(giftId);
                ApplyGift(player, gift, cardsDb, itemsDb);
                gift.AcceptedByPlayer(player);
            }

            player.Save();

            responseBody = Serialize(new Response
            {
                accepted = true,
                completed = true,
                gifts = player.receivedGifts
                    .Where(Gift.Exists)
                    .Select(Gift.Load)
                    .ToList()
            });
            SetBasicResponseHeaders();
            return Task.CompletedTask;
        }

        private IEnumerable<long> GetTargetGiftIds(PlayerProfile player)
        {
            if (pathParameters.ContainsKey("gift_id"))
            {
                long giftId = long.Parse(GetPathParameter("gift_id"));
                if (player.acceptedGifts.Contains(giftId))
                    return Enumerable.Empty<long>();

                if (!player.receivedGifts.Contains(giftId))
                    throw new APIErrorException("A1321", $"Unknown received gift {giftId}.");

                return new[] { giftId };
            }

            return player.receivedGifts.ToList();
        }

        private static void ApplyGift(
            PlayerProfile player,
            Gift gift,
            CardsContext cardsDb,
            ItemsContext itemsDb)
        {
            switch ((ItemCategory) gift.item_category_id)
            {
                case ItemCategory.Card:
                    player.GrantCard(gift.item_id, gift.quantity, cardsDb, itemsDb);
                    break;
                case ItemCategory.Accessory:
                    player.GrantAccessory(gift.item_id, gift.quantity);
                    break;
                case ItemCategory.EnhancementItem:
                    GrantStackableItem(player.items.enhancement, gift.item_id, gift.quantity);
                    break;
                case ItemCategory.EvolutionItem:
                case ItemCategory.EvolutionItemSmall:
                case ItemCategory.EvolutionItemMiddle:
                case ItemCategory.EvolutionItemBig:
                    GrantStackableItem(player.items.evolution, gift.item_id, gift.quantity);
                    break;
                case ItemCategory.StaminaItem:
                    GrantStackableItem(player.items.stamina, gift.item_id, gift.quantity);
                    break;
                case ItemCategory.GachaTicket:
                    GrantStackableItem(player.items.gachaTickets, gift.item_id, gift.quantity);
                    break;
                case ItemCategory.GameCoinPoint:
                    player.data.money += gift.quantity;
                    break;
                case ItemCategory.FriendPoint:
                    player.data.friendPoint += gift.quantity;
                    break;
                case ItemCategory.BillingPoint:
                    player.data.freeBlessing += gift.quantity;
                    break;
                case ItemCategory.BraveCoin:
                    player.data.braveCoin += gift.quantity;
                    break;
                case ItemCategory.ExchangePoint:
                    player.data.exchangePoint += gift.quantity;
                    break;
                default:
                    Utils.LogWarning($"Gift {gift.id} uses unsupported item category {gift.item_category_id}; marking it accepted without inventory mutation.");
                    break;
            }
        }

        private static void GrantStackableItem(IDictionary<long, long> playerItems, long masterId, int quantity)
        {
            if (!playerItems.TryGetValue(masterId, out long itemId) || !Item.Exists(itemId))
            {
                Item newItem = new()
                {
                    id = Item.GetID(),
                    master_id = masterId,
                    quantity = quantity
                };
                playerItems[masterId] = newItem.id;
                newItem.Save();
                return;
            }

            Item existingItem = Item.Load(itemId);
            existingItem.quantity += quantity;
            existingItem.Save();
        }

        public class Response
        {
            public bool accepted { get; set; }
            public bool completed { get; set; }
            public IList<Gift> gifts { get; set; } = new List<Gift>();
        }
    }
}
