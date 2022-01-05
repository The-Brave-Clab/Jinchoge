using System.Text;
using Newtonsoft.Json;

namespace Yuyuyui.PrivateServer
{
    public class PlayerProfile : UserDataBase<PlayerProfile>
    {
        public ID id { get; set; } = new();
        public Profile profile { get; set; } = new();
        public Data data { get; set; } = new();

        public IDictionary<int, IList<int>> newAlbum = new Dictionary<int, IList<int>>();

        public IList<long> accessories = new List<long>();

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
        }
    }
}