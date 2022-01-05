using System.Text;
using Newtonsoft.Json;

namespace Yuyuyui.PrivateServer
{
    public class PlayerProfile
    {
        public ID id { get; set; } = new();
        public Profile profile { get; set; } = new();
        public Data data { get; set; } = new();

        public void Save()
        {
            string file = Path.Combine(PrivateServer.EnsurePlayerDataFolder(id.code), $"{id.code}.json");
            File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static PlayerProfile Load(string code)
        {
            string file = Path.Combine(PrivateServer.DATA_FOLDER, code, $"{code}.json");
            string content = File.ReadAllText(file, Encoding.UTF8);
            return JsonConvert.DeserializeObject<PlayerProfile>(content)!;
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
            public int regulationVersion { get; set; } = 0;
            public int tutorialProgress { get; set; } = 0;
        }
    }
}