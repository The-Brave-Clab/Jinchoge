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

        public IDictionary<string, CharacterFamiliarity> characterFamiliarities { get; set; } 
            = new Dictionary<string, CharacterFamiliarity>();

        public IList<string> friends { get; set; } = new List<string>(); // friend user id
        public IList<long> friendRequests { get; set; } = new List<long>(); // request id

        public IList<long> receivedGifts { get; set; } = new List<long>();
        public IList<long> acceptedGifts { get; set; } = new List<long>();

        public Progress progress { get; set; } = new();

        public IDictionary<long, IList<int>> gachaSelections { get; set; } 
            = new Dictionary<long, IList<int>>();

        protected override string Identifier => id.code;
        
        


        public CharacterFamiliarity GetCharacterFamiliarity(long characterId1, long characterId2)
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
            
            Save();

            return characterFamiliarities[groupName];
        }

        public void BanAccount()
        {
            // Delete all BasePlayerData.Identifier related entries
            
            Utils.LogWarning($"Banning Account {id.code}");
            
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
            
            Utils.LogWarning($"Account {id.code} Banned");
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
            public int taishaPoint { get; set; } = 0;

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
        }
    }
}