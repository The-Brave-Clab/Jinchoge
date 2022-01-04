using System.Text;
using Newtonsoft.Json;

namespace Yuyuyui.PrivateServer
{
    public class PlayerProfile
    {
        public string uuid { get; set; } = "";
        public string code { get; set; } = "";
        public string nickname { get; set; } = "";
        public string comment { get; set; } = "";
        public int regulationVersion { get; set; }
        public int tutorialProgress { get; set; }

        public void Save()
        {
            string file = Path.Combine(PrivateServer.EnsurePlayerDataFolder(code), $"{code}.json");
            File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static PlayerProfile Load(string code)
        {
            string file = Path.Combine(PrivateServer.DATA_FOLDER, code, $"{code}.json");
            string content = File.ReadAllText(file, Encoding.UTF8);
            return JsonConvert.DeserializeObject<PlayerProfile>(content)!;
        }
    }
}