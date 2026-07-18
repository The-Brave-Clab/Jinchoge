using System.Collections.Generic;
using System.Linq;
using Yuyuyui.PrivateServer.DataModel;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer
{
    public class PlayerProfile : BasePlayerData<PlayerProfile, string>
    {
        public ID id { get; set; } = new();
        public Profile profile { get; set; } = new();
        public Data data { get; set; } = new();

        public IDictionary<int, IList<int>> newAlbum { get; set; } = new Dictionary<int, IList<int>>();

        public IDictionary<long, long> accessories { get; set; } = new Dictionary<long, long>(); // master_id, id

        public IList<long> clubOrders { get; set; } = new List<long>();
        public IList<long> clubWorkingSlots { get; set; } = new List<long>();

        public IDictionary<long, long> cards { get; set; } = new Dictionary<long, long>(); // base_card_master_id, id

        public IList<long> decks { get; set; } = new List<long>();

        public Items items { get; set; } = new();

        public IDictionary<string, CharacterFamiliarityWithAssist> characterFamiliarities { get; set; } 
            = new Dictionary<string, CharacterFamiliarityWithAssist>();

        public IList<string> friends { get; set; } = new List<string>(); // friend user id
        public IList<long> friendRequests { get; set; } = new List<long>(); // request id

        public IList<long> receivedGifts { get; set; } = new List<long>();
        public IList<long> acceptedGifts { get; set; } = new List<long>();

        public Progress progress { get; set; } = new();

        public IDictionary<long, IList<int>> gachaSelections { get; set; } 
            = new Dictionary<long, IList<int>>();

        public Transactions transactions { get; set; } = new();

        protected override string Identifier => id.code;
        
        


        public CharacterFamiliarityWithAssist GetCharacterFamiliarity(long characterId1, long characterId2)
        {
            string groupName = CharacterFamiliarityWithAssist.GetGroupName(characterId1, characterId2);
            if (!characterFamiliarities.ContainsKey(groupName))
                characterFamiliarities.Add(groupName, new()
                {
                    character_group = groupName,
                    familiarity = 0,
                    rank = 1,
                    assist_level = 1
                });
            
            Save();

            return characterFamiliarities[groupName];
        }

        public void BanAccount()
        {
            // Delete all BasePlayerData.Identifier related entries
            
            Utils.LogWarning(string.Format(Resources.LOG_PS_ACCOUNT_BANNING, id.code));
            
            accessories.Values.ForEach(Accessory.Delete);
            clubOrders.ForEach(ClubOrder.Delete);
            clubWorkingSlots.ForEach(ClubWorkingSlot.Delete);
            cards.Values.ForEach(Card.Delete);
            friendRequests.ForEach(FriendRequest.Delete);
            
            // For decks, remove the units inside first
            foreach (var deckId in decks)
            {
                Deck deck = Deck.Load(deckId);
                deck.units.ForEach(Unit.Delete);
                deck.Delete();
            }
            
            receivedGifts.ForEach(Gift.Delete);
            acceptedGifts.ForEach(Gift.Delete);

            progress.chapters.Values.ForEach(ChapterProgress.Delete);
            progress.episodes.Values.ForEach(EpisodeProgress.Delete);
            progress.stages.Values.ForEach(StageProgress.Delete);
            
            items.autoClearTickets.Values.ForEach(Item.Delete);
            items.enhancement.Values.ForEach(Item.Delete);
            items.eventItems.Values.ForEach(Item.Delete);
            items.evolution.Values.ForEach(Item.Delete);
            items.stamina.Values.ForEach(Item.Delete);
            
            // Finally, delete ourselves
            PrivateServer.RemovePlayerProfile(this);
            Delete();
            
            Utils.LogWarning(string.Format(Resources.LOG_PS_ACCOUNT_BANNED, id.code));
        }

        // Accessory only, skip the gift acceptance progress
        public void UpsertPotentialGift(int previousPotential, int currentPotential, DataModel.Card masterCard)
        {
            var border = masterCard.PotentialGiftBorder;
            long? potentialGiftId = masterCard.PotentialGiftId;
            if (border == null || potentialGiftId == null)
                return;

            if (previousPotential >= border || currentPotential < border)
                return;

            DataModel.Gift? masterGift;
            using (var giftsDb = new GiftsContext())
                masterGift = giftsDb.Gifts
                    .Where(gift => gift.ContentType == "Accessory")
                    .FirstOrDefault(gift => gift.Id == potentialGiftId);

            if (masterGift == null)
            {
                Utils.LogWarning($"Potential gift {potentialGiftId} was not found; skipping accessory grant.");
                return;
            }

            GrantAccessory(masterGift.ContentId, masterGift.Quantity);
        }

        public void GrantAccessory(long? accessoryId, int quantity)
        {
            if (accessoryId == null) return;

            bool isNew = !accessories.Keys.Contains((long) accessoryId);

            Accessory accessory;
            if (isNew)
            {
                accessory = Accessory.NewAccessoryByMasterId((long) accessoryId);
                accessory.quantity = quantity - 1;

                accessories.Add((long) accessoryId, accessory.id);
                accessory.Save();
                Save();

                Utils.Log(string.Format(Resources.LOG_PS_ACCESSORY_ASSIGN_NEW, (long) accessoryId));
                return;
            }

            accessory = Accessory.Load(accessories[(long)accessoryId]);
            accessory.quantity += quantity;

            accessory.Save();

            Utils.Log(string.Format(Resources.LOG_PS_ACCESSORY_QUANTITY_INCREASED, accessoryId, quantity));
        }

        public long GetCardProfileKey(long masterCardId, CardsContext cardsDb)
        {
            Card lookupCard = Card.NewCardByMasterId(masterCardId);
            return lookupCard.MasterData(cardsDb).BaseCardId;
        }

        public long NormalizeCardProfileKey(long masterCardId, CardsContext cardsDb)
        {
            long cardProfileKey = GetCardProfileKey(masterCardId, cardsDb);
            if (cardProfileKey != masterCardId && !cards.ContainsKey(cardProfileKey) && cards.TryGetValue(masterCardId, out long oldUserCardId))
            {
                cards[cardProfileKey] = oldUserCardId;
                cards.Remove(masterCardId);
                Save();
            }

            return cardProfileKey;
        }

        public bool HasCardForMasterId(long masterCardId, CardsContext cardsDb)
        {
            long cardProfileKey = GetCardProfileKey(masterCardId, cardsDb);
            return cards.ContainsKey(cardProfileKey) || cards.ContainsKey(masterCardId);
        }

        public long GetUserCardIdForMasterId(long masterCardId, CardsContext cardsDb)
        {
            long cardProfileKey = NormalizeCardProfileKey(masterCardId, cardsDb);
            return cards[cardProfileKey];
        }

        public void GrantCard(long masterCardId, int potentialCount, CardsContext cardsDb, ItemsContext itemsDb)
        {
            long cardProfileKey = NormalizeCardProfileKey(masterCardId, cardsDb);
            bool isNewCard = !cards.Keys.Contains(cardProfileKey);

            Card card;
            if (isNewCard)
            {
                card = Card.NewCardByMasterId(masterCardId);
                cards.Add(cardProfileKey, card.id);
                card.Save();
                Save();
                potentialCount -= 1;
                Utils.Log(string.Format(Resources.LOG_PS_CARD_ASSIGN_NEW, masterCardId));
            }
            else
            {
                card = Card.Load(cards[cardProfileKey]);
            }

            int previousPotentialCount = card.potential;
            card.AddPotential(potentialCount);

            DataModel.Card masterCard = card.MasterData(cardsDb);
            UpsertPotentialGift(previousPotentialCount, card.potential, masterCard);

            card = Card.Load(cards[cardProfileKey]);
            UpdateEvolutionAccessoriesForCard(card, potentialCount, cardsDb);

            EnsureEligibleCardTitle(cardsDb, itemsDb);
        }

        public IList<int> EnsureEligibleCardTitle(CardsContext cardsContext, ItemsContext itemsContext)
        {
            var eligibleCardTitleItems = GetObtainableTitles(cardsContext, itemsContext);
            if (!eligibleCardTitleItems.Any()) return new List<int>();

            eligibleCardTitleItems.ForEach(titleItem => items.titleItems.Add(titleItem.Id));
            items.titleItems = items.titleItems
                .Concat(eligibleCardTitleItems.Select(ti => ti.Id))
                .ToList();

            Save();

            return eligibleCardTitleItems.Select(ti => (int) ti.Id).ToList();
        }

        private void UpdateEvolutionAccessoriesForCard(
            Card playerCard, int potentialCount, CardsContext cardsDb)
        {
            if (playerCard.evolution_level < 1 || potentialCount < 1)
                return;
        
            cardsDb.Cards
                .Where(card => card.Id == playerCard.master_id)
                .ForEach(card => UpdateEvolutionAccessories(potentialCount, card));
        }

        private void UpdateEvolutionAccessories(int potentialCount, DataModel.Card card)
        {
            GrantAccessory(card.EvolutionRewardAccessory1Id, potentialCount);
            GrantAccessory(card.EvolutionRewardAccessory2Id, potentialCount);
            GrantAccessory(card.EvolutionRewardAccessory3Id, potentialCount);
            GrantAccessory(card.EvolutionRewardAccessory4Id, potentialCount);
            GrantAccessory(card.EvolutionRewardAccessory5Id, potentialCount);
        }
        
        
        private const int MINIMAL_CARD_POTENTIAL = 1;
        private const int MINIMAL_CARD_LEVEL = 99;
        private const int MINIMAL_EVOLUTION_LEVEL = 5;
        private const int CARD_TITLE_CONTENT_TYPE = 2;
        private static readonly List<int> ELIGIBLE_RARITY_LIST = new() { 400, 450, 500 };
    
        private IQueryable<TitleItem> GetObtainableTitles(CardsContext cardsContext, ItemsContext itemsContext)
        {
            var userEligibleCardIdList = GetBaseCardIdsEligibleForObtainingTitle(cardsContext);
            return GetObtainableTitleItems(itemsContext, userEligibleCardIdList);
        }

        private IQueryable<TitleItem> GetObtainableTitleItems(ItemsContext itemsContext, IEnumerable<long> userEligibleCardIdList)
        {
            return itemsContext.TitleItems
                .Where(titleItem => !items.titleItems.Contains(titleItem.Id))
                .Where(titleItem => titleItem.ContentType == CARD_TITLE_CONTENT_TYPE && titleItem.Priority != null)
                .Where(titleItem => userEligibleCardIdList.Contains(titleItem.Priority.GetValueOrDefault(0)));
        }

        private IEnumerable<long> GetBaseCardIdsEligibleForObtainingTitle(CardsContext cardsContext)
        {
            return cards.Values
                .Select(Card.Load)
                .Where(card => ELIGIBLE_RARITY_LIST.Contains(card.MasterData(cardsContext).Rarity))
                .Where(card => card.potential >= MINIMAL_CARD_POTENTIAL)
                .Where(card => card.level >= MINIMAL_CARD_LEVEL)
                .Where(card => card.evolution_level >= MINIMAL_EVOLUTION_LEVEL)
                .Select(card => card.MasterData(cardsContext).BaseCardId);
        }
        

        public class ID
        {
            public string uuid { get; set; } = "";
            public string code { get; set; } = "";
        }

        public class Profile
        {
            public string nickname { get; set; } = "";
            public string comment { get; set; } = "";
        }

        public class Data
        {
            public int level { get; set; } = 1;
            public long exp { get; set; } = 0;
            public int regulationVersion { get; set; } = 0;
            public int tutorialProgress { get; set; } = 0;

            public int paidBlessing { get; set; } = 999900; // we only do fixed paid in private server
            public int freeBlessing { get; set; } = 0;
            public long money { get; set; } = 0;
            public int friendPoint { get; set; } = 0;
            public int braveCoin { get; set; } = 0;
            public int exchangePoint { get; set; } = 999999;

            public long? titleItemID { get; set; } = null;
            public int stamina { get; set; } = 140; // wip
            public int weekdayStamina { get; set; } = 6;
            public int enhancementItemCapacity { get; set; } = 590;
            public bool birthdateRegistered { get; set; } = false;

            public long lastActive { get; set; } = 0; // unixtime
        }

        public class Progress
        {
            public IDictionary<long, long> chapters { get; set; } = new Dictionary<long, long>(); // master_id, id
            public IDictionary<long, long> episodes { get; set; } = new Dictionary<long, long>(); // master_id, id
            public IDictionary<long, long> stages { get; set; } = new Dictionary<long, long>(); // master_id, id
            public IList<long> adventureBooksRead { get; set; } = new List<long>(); // adventure_book_id
        }

        public class Items
        {
            public IDictionary<long, long> autoClearTickets { get; set; } = new Dictionary<long, long>(); // master_id, id
            public IDictionary<long, long> enhancement { get; set; } = new Dictionary<long, long>(); // master_id, id
            public IDictionary<long, long> eventItems { get; set; } = new Dictionary<long, long>(); // master_id, id
            public IDictionary<long, long> evolution { get; set; } = new Dictionary<long, long>(); // master_id, id
            public IDictionary<long, long> stamina { get; set; } = new Dictionary<long, long>(); // master_id, id
            public IDictionary<long, long> gachaTickets { get; set; } = new Dictionary<long, long>(); // master_id, id
            public IList<long> titleItems { get; set; } = new List<long>(); // master_id
        }

        public class Transactions
        {
            public IDictionary<long, long> questTransactions { get; set; } = new Dictionary<long, long>(); // stage_id, id
            public IDictionary<long, ShopProductTransaction> shopProductTransactions { get; set; }
                = new Dictionary<long, ShopProductTransaction>(); // transaction_id, shop/product pair
            public IDictionary<long, GachaTransaction> gachaTransactions { get; set; }
                = new Dictionary<long, GachaTransaction>(); // transaction_id, gacha transaction state
            public IDictionary<long, BillingPointTransaction> billingPointTransactions { get; set; }
                = new Dictionary<long, BillingPointTransaction>(); // transaction_id, billing point shop state
            public IDictionary<long, StaminaItemTransaction> staminaItemTransactions { get; set; }
                = new Dictionary<long, StaminaItemTransaction>(); // transaction_id, stamina item state
            public IDictionary<string, int> shopProductPurchaseCounts { get; set; }
                = new Dictionary<string, int>(); // shop_id/product_id, purchased count
        }

        public class ShopProductTransaction
        {
            public string shop_id { get; set; } = "";
            public long product_id { get; set; }
            public bool completed { get; set; }
        }

        public class BillingPointTransaction
        {
            public string kind { get; set; } = "";
            public bool completed { get; set; }
        }

        public class StaminaItemTransaction
        {
            public long stamina_item_id { get; set; }
            public bool completed { get; set; }
        }

        public class GachaTransaction
        {
            public long gacha_id { get; set; }
            public long lineup_id { get; set; }
            public bool completed { get; set; }
            public IList<GachaTransactionResult> results { get; set; } = new List<GachaTransactionResult>();
        }

        public class GachaTransactionResult
        {
            public long content_id { get; set; }
            public bool is_new { get; set; }
        }
    }
}
