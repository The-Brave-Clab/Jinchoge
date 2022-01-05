using System.Text;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Yuyuyui.PrivateServer
{
    public abstract class BaseUserData<T> where T : BaseUserData<T>
    {
        protected abstract string Identifier { get; }

        private static string GetFileName(string identifier)
        {
            return Path.Combine(EnsurePlayerDataFolder(typeof(T).Name), $"{identifier}.yaml");
        }

        public void Save()
        {
            string file = GetFileName(Identifier);
            //File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
            var serializer = new Serializer();
            File.WriteAllText(file, serializer.Serialize(this));
        }

        public static T Load(string identifier)
        {
            string file = GetFileName(identifier);
            string content = File.ReadAllText(file, Encoding.UTF8);
            //return JsonConvert.DeserializeObject<T>(content)!;
            var deserializer = new Deserializer();
            return deserializer.Deserialize<T>(content);
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