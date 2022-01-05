using System.Text;
using Newtonsoft.Json;

namespace Yuyuyui.PrivateServer
{
    public abstract class UserDataBase<T> where T : UserDataBase<T>
    {
        protected abstract string Identifier { get; }

        private static string GetFileName(string identifier)
        {
            return Path.Combine(EnsurePlayerDataFolder(typeof(T).Name), $"{identifier}.json");
        }

        public void Save()
        {
            string file = GetFileName(Identifier);
            File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static T Load(string identifier)
        {
            string file = GetFileName(identifier);
            string content = File.ReadAllText(file, Encoding.UTF8);
            return JsonConvert.DeserializeObject<T>(content)!;
        }

        public static bool Exists(string identifier)
        {
            return File.Exists(GetFileName(identifier));
        }

        private static string EnsurePlayerDataFolder(string subFolder)
        {
            string dir = Path.Combine(PrivateServer.DATA_FOLDER, subFolder);
            return Utils.EnsureDirectory(dir);
        }
    }
}