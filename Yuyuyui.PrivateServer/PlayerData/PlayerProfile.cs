using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        
        


        public async Task<CharacterFamiliarityWithAssist> GetCharacterFamiliarity(long characterId1, long characterId2)
        {
            string groupName = CharacterFamiliarity.GetGroupName(characterId1, characterId2);
            if (!characterFamiliarities.ContainsKey(groupName))
                characterFamiliarities.Add(groupName, new()
                {
                    character_group = groupName,
                    familiarity = 0,
                    rank = 1,
                    assist_level = 1
                });
            
            await Save();

            return characterFamiliarities[groupName];
        }

        public async Task BanAccount()
        {
            // Delete all BasePlayerData.Identifier related entries
            
            Utils.LogWarning(string.Format(Resources.LOG_PS_ACCOUNT_BANNING, id.code));
            
            await accessories.Values.ForEachAsync(Accessory.Delete);
            await clubOrders.ForEachAsync(ClubOrder.Delete);
            await clubWorkingSlots.ForEachAsync(ClubWorkingSlot.Delete);
            await cards.Values.ForEachAsync(Card.Delete);
            await friendRequests.ForEachAsync(FriendRequest.Delete);
            
            // For decks, remove the units inside first
            foreach (var deckId in decks)
            {
                Deck deck = await Deck.Load(deckId);
                await deck.units.ForEachAsync(Unit.Delete);
                await deck.Delete();
            }
            
            await receivedGifts.ForEachAsync(Gift.Delete);
            await acceptedGifts.ForEachAsync(Gift.Delete);

            await progress.chapters.Values.ForEachAsync(ChapterProgress.Delete);
            await progress.episodes.Values.ForEachAsync(EpisodeProgress.Delete);
            await progress.stages.Values.ForEachAsync(StageProgress.Delete);
            
            await items.autoClearTickets.Values.ForEachAsync(Item.Delete);
            await items.enhancement.Values.ForEachAsync(Item.Delete);
            await items.eventItems.Values.ForEachAsync(Item.Delete);
            await items.evolution.Values.ForEachAsync(Item.Delete);
            await items.stamina.Values.ForEachAsync(Item.Delete);
            
            // Finally, delete ourselves
            await PrivateServer.RemovePlayerProfile(this);
            await Delete();
            
            Utils.LogWarning(string.Format(Resources.LOG_PS_ACCOUNT_BANNED, id.code));
        }

        // Accessory only, skip the gift acceptance progress
        public async Task UpsertPotentialGift(int previousPotential, int currentPotential, DataModel.Card masterCard)
        {
            var border = masterCard.PotentialGiftBorder;
            if (previousPotential >= border || currentPotential < border)
                return;
        
            long? potentialGiftId = masterCard.PotentialGiftId;

            DataModel.Gift masterGift;
            await using (GiftsContext giftsDb = new())
            {
                masterGift = giftsDb.Gifts
                    .Where(gift => gift.ContentType == "Accessory")
                    .First(gift => gift.Id == potentialGiftId);
            }

            await GrantAccessory(masterGift.ContentId, masterGift.Quantity);
        }

        public async Task GrantAccessory(long? accessoryId, int quantity)
        {
            if (accessoryId == null) return;

            bool isNew = !accessories.Keys.Contains((long) accessoryId);

            Accessory accessory;
            if (isNew)
            {
                accessory = await Accessory.NewAccessoryByMasterId((long) accessoryId);
                accessory.quantity = quantity - 1;

                accessories.Add((long) accessoryId, accessory.id);
                await accessory.Save();
                await Save();

                Utils.Log(string.Format(Resources.LOG_PS_ACCESSORY_ASSIGN_NEW, (long) accessoryId));
                return;
            }

            accessory = await Accessory.Load(accessories[(long)accessoryId]);
            accessory.quantity += quantity;

            await accessory.Save();

            Utils.Log(string.Format(Resources.LOG_PS_ACCESSORY_QUANTITY_INCREASED, accessoryId, quantity));
        }

        public async Task GrantCard(long masterCardId, int potentialCount)
        {
            bool isNewCard = !cards.Keys.Contains(masterCardId);

            Card card;
            if (isNewCard)
            {
                card = await Card.NewCardByMasterId(masterCardId);
                cards.Add(masterCardId, card.id);
                await card.Save();
                await Save();
                potentialCount -= 1;
                Utils.Log(string.Format(Resources.LOG_PS_CARD_ASSIGN_NEW, masterCardId));
            }
            else
            {
                card = await Card.Load(cards[masterCardId]);
            }

            int previousPotentialCount = card.potential;
            await card.AddPotential(potentialCount);

            DataModel.Card masterCard;
            await using (CardsContext cardsDb = new())
                masterCard = cardsDb.Cards.First(c => c.Id == masterCardId);
            await UpsertPotentialGift(previousPotentialCount, card.potential, masterCard);

            card = await Card.Load(cards[masterCardId]);
            await UpdateEvolutionAccessoriesForCard(card, potentialCount);

            await EnsureEligibleCardTitle();
        }

        public async Task<IList<int>> EnsureEligibleCardTitle()
        {
            var eligibleCardTitleItems = await GetObtainableTitles();
            if (!eligibleCardTitleItems.Any()) return new List<int>();

            eligibleCardTitleItems.ForEach(titleItem => items.titleItems.Add(titleItem.Id));
            items.titleItems = items.titleItems
                .Concat(eligibleCardTitleItems.Select(ti => ti.Id))
                .ToList();

            await Save();

            return eligibleCardTitleItems.Select(ti => (int) ti.Id).ToList();
        }

        private async Task UpdateEvolutionAccessoriesForCard(Card playerCard, int potentialCount)
        {
            if (playerCard.evolution_level < 1 || potentialCount < 1)
                return;

            List<DataModel.Card> cardsQuery;
            await using (CardsContext cardsDb = new())
            {
                cardsQuery = cardsDb.Cards
                    .Where(card => card.Id == playerCard.master_id)
                    .ToList();
            }
            await cardsQuery.ForEachAsync(card => UpdateEvolutionAccessories(potentialCount, card));
        }

        private async Task UpdateEvolutionAccessories(int potentialCount, DataModel.Card card)
        {
            await GrantAccessory(card.EvolutionRewardAccessory1Id, potentialCount);
            await GrantAccessory(card.EvolutionRewardAccessory2Id, potentialCount);
            await GrantAccessory(card.EvolutionRewardAccessory3Id, potentialCount);
            await GrantAccessory(card.EvolutionRewardAccessory4Id, potentialCount);
            await GrantAccessory(card.EvolutionRewardAccessory5Id, potentialCount);
        }
        
        
        private const int MINIMAL_CARD_POTENTIAL = 1;
        private const int MINIMAL_CARD_LEVEL = 99;
        private const int MINIMAL_EVOLUTION_LEVEL = 5;
        private const int CARD_TITLE_CONTENT_TYPE = 2;
        private static readonly List<int> ELIGIBLE_RARITY_LIST = new() { 400, 450, 500 };
    
        private async Task<List<TitleItem>> GetObtainableTitles()
        {
            var userEligibleCardIdList = await GetBaseCardIdsEligibleForObtainingTitle();
            return GetObtainableTitleItems(userEligibleCardIdList);
        }

        private List<TitleItem> GetObtainableTitleItems(IEnumerable<long> userEligibleCardIdList)
        {
            using ItemsContext itemsContext = new();
            return itemsContext.TitleItems
                .Where(titleItem => !items.titleItems.Contains(titleItem.Id))
                .Where(titleItem => titleItem.ContentType == CARD_TITLE_CONTENT_TYPE && titleItem.Priority != null)
                .ToList() // execute query
                .Where(titleItem => userEligibleCardIdList.Contains(titleItem.Priority.GetValueOrDefault(0)))
                .ToList();
        }

        private async Task<IEnumerable<long>> GetBaseCardIdsEligibleForObtainingTitle()
        {
            var baseCards = await cards.Values
                .Select(Card.Load)
                .WhenAll();
            return baseCards
                .Where(card => ELIGIBLE_RARITY_LIST.Contains(card.MasterData().Rarity))
                .Where(card => card.potential >= MINIMAL_CARD_POTENTIAL)
                .Where(card => card.level >= MINIMAL_CARD_LEVEL)
                .Where(card => card.evolution_level >= MINIMAL_EVOLUTION_LEVEL)
                .Select(card => card.MasterData().BaseCardId);
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
            public IList<long> titleItems { get; set; } = new List<long>(); // master_id
        }

        public class Transactions
        {
            public IDictionary<long, long> questTransactions { get; set; } = new Dictionary<long, long>(); // stage_id, id
        }
    }
}