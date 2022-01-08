using System.Text;
using YamlDotNet.Serialization;

namespace Yuyuyui.PrivateServer
{
    public abstract class BasePlayerData<TSelf, TIdentifier> 
        where TSelf : BasePlayerData<TSelf, TIdentifier>
        where TIdentifier : notnull
    {
        protected abstract TIdentifier Identifier { get; }

        private static readonly Dictionary<TIdentifier, TSelf> Cache = new();

        private static string GetFileName(TIdentifier identifier)
        {
            return Path.Combine(EnsurePlayerDataFolder(typeof(TSelf).Name), $"{identifier}.yaml");
        }

        public void Save()
        {
            string file = GetFileName(Identifier);
            //File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
            var serializer = new Serializer();
            File.WriteAllText(file, serializer.Serialize(this));
            
            // Update cache
            if (!Cache.ContainsKey(Identifier))
            {
                Cache.Add(Identifier, (TSelf) this);
            }
        }

        public static TSelf Load(TIdentifier identifier)
        {
            // Get directly from cache if possible
            if (Cache.ContainsKey(identifier))
            {
                return Cache[identifier];
            }
            
            string file = GetFileName(identifier);
            string content = File.ReadAllText(file, Encoding.UTF8);
            //return JsonConvert.DeserializeObject<T>(content)!;
            var deserializer = new Deserializer();
            TSelf result = deserializer.Deserialize<TSelf>(content);

            // Add to cache
            Cache.Add(identifier, result);

            return result;
        }

        public static bool Exists(TIdentifier identifier)
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