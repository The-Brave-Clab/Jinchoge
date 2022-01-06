namespace Yuyuyui.PrivateServer
{
    public class PlayerProfile : BaseUserData<PlayerProfile>
    {
        public ID id { get; set; } = new();
        public Profile profile { get; set; } = new();
        public Data data { get; set; } = new();

        public IDictionary<int, IList<int>> newAlbum { get; set; } = new Dictionary<int, IList<int>>();

        public IList<long> accessories { get; set; } = new List<long>();

        public IList<long> clubOrders { get; set; } = new List<long>();

        public IList<long> cards { get; set; } = new List<long>();

        public IList<long> evolutionItems = new List<long>();

        public IList<CharacterFamiliarity> characterFamiliarities { get; set; } = new List<CharacterFamiliarity>();

        protected override string Identifier => id.code;

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
            public int regulationVersion { get; set; } = 0;
            public int tutorialProgress { get; set; } = 0;

            public int paidBlessing { get; set; } = 1000000; // we only do fixed paid in private server
        }
    }
}