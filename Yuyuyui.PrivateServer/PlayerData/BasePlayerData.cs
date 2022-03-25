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

        private static readonly Dictionary<TIdentifier, object> locks = new();

        private static object GetLockObj(TIdentifier identifier)
        {
            lock (locks)
            {
                if (locks.ContainsKey(identifier))
                    return locks[identifier];

                object lockObj = new object();
                locks.Add(identifier, lockObj);
                return lockObj;
            }
        }

        private object GetLockObj()
        {
            return GetLockObj(Identifier);
        }

        private static string GetFileName(TIdentifier identifier)
        {
            return Path.Combine(EnsurePlayerDataFolder(typeof(TSelf).Name), $"{identifier}.yaml");
        }

        public void Save()
        {
            lock (GetLockObj())
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
        }

        public static TSelf Load(TIdentifier identifier)
        {
            lock (Cache)
            {
                // Get directly from cache if possible
                if (Cache.ContainsKey(identifier))
                {
                    return Cache[identifier];
                }

                lock (GetLockObj(identifier))
                {
                    string file = GetFileName(identifier);
                    string content = File.ReadAllText(file, Encoding.UTF8);
                    //return JsonConvert.DeserializeObject<T>(content)!;
                    var deserializer = new Deserializer();
                    TSelf result = deserializer.Deserialize<TSelf>(content);

                    // Add to cache
                    Cache.Add(identifier, result);

                    return result;
                }
            }
        }

        public static void Delete(TIdentifier identifier)
        {
            lock (Cache)
                Cache.Remove(identifier);
            lock (GetLockObj(identifier))
                File.Delete(GetFileName(identifier));
        }

        public void Delete()
        {
            Delete(Identifier);
        }

        public static bool Exists(TIdentifier identifier)
        {
            lock (GetLockObj(identifier))
                return File.Exists(GetFileName(identifier));
        }

        private static string EnsurePlayerDataFolder(string subFolder)
        {
            string dir = Path.Combine(PrivateServer.PLAYER_DATA_FOLDER, subFolder);
            return Utils.EnsureDirectory(dir);
        }
    }
}