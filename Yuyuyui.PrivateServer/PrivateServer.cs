namespace Yuyuyui.PrivateServer
{
    public static class PrivateServer
    {
        public struct PlayerSession
        {
            public PlayerProfile player;
            public string sessionID;
            public string sessionKey;
            public DeviceInfo deviceInfo;
        }

        public struct DeviceInfo
        {
            public enum OS
            {
                Android,
                iOS
            }

            public OS os;
            public string platformName;
            public string unityVersion;
            public string appVersion;
            public string deviceName;
            public string userAgent;
        }

        private static string dataFolder;

        private static Dictionary<string, PlayerProfile> playerUUID;
        private static Dictionary<string, PlayerProfile> playerCode;
        private static Dictionary<string, PlayerSession> playerSessions;

        public const string YUYUYUI_APP_VERSION = "3.16.0";

        public const string PLAYER_DATA_FOLDER = "PlayerData";
        public const string PLAYER_DATA_FILE = "players.dat";

        public const string LOCAL_DATA_FOLDER = "Resources";
        public const string LOCAL_DATA_VERSION_FILE = "master_data.version.json";

        public const string OFFICIAL_API_SERVER = "app.yuyuyui.jp";
        public const string PRIVATE_LOCAL_API_SERVER = "private.yuyuyui.org";
        public const string PRIVATE_PUBLIC_API_SERVER = "936fkiz1v2.execute-api.ap-northeast-1.amazonaws.com";

        private static object dataFileLock = new();
        
        public static readonly HttpClient HttpClient = new();

        static PrivateServer()
        {
            playerUUID = new Dictionary<string, PlayerProfile>();
            playerCode = new Dictionary<string, PlayerProfile>();
            playerSessions = new Dictionary<string, PlayerSession>();


            lock (dataFileLock)
            {
                dataFolder = Utils.EnsureDirectory(PLAYER_DATA_FOLDER);
            }

            var playerDataFile = Path.Combine(dataFolder, PLAYER_DATA_FILE);

            lock(dataFileLock)
            {
                if (!File.Exists(playerDataFile))
                {
                    File.AppendAllText(playerDataFile, null);
                }
            }

            IEnumerable<string> players;
            lock (dataFileLock)
                players = File.ReadLines(playerDataFile);
            
            foreach (var s in players)
            {
                var split = s.Split(',');
                string uuid = split[0];
                string code = split[1];
                PlayerProfile player = PlayerProfile.Load(code);
                playerUUID.Add(player.id.uuid, player);
                playerCode.Add(player.id.code, player);
            }
        }

        private static PlayerProfile RegisterNewPlayer(string uuid)
        {
            string newCode = Utils.GenerateRandomDigit(10);
            while (PlayerProfile.Exists(newCode))
            {
                newCode = Utils.GenerateRandomDigit(10);
            }
            
            var player = new PlayerProfile
            {
                id = new()
                {
                    uuid = uuid, 
                    code = newCode
                }
            };
            playerUUID.Add(player.id.uuid, player);
            playerCode.Add(player.id.code, player);

            var playerDataFile = Path.Combine(dataFolder, PLAYER_DATA_FILE);
            lock (dataFileLock)
                File.AppendAllText(playerDataFile, $"{player.id.uuid},{player.id.code}\n");
            player.Save();

            Utils.Log($"Registered new player {player.id.code}");

            return player;
        }

        public static PlayerSession CreateSessionForPlayer(string uuid, EntityBase entity)
        {
            PlayerSession session;
            string verb = "";
            try
            {
                session = playerSessions.First(p => p.Value.player.id.uuid == uuid).Value;
                verb = "Found";
            }
            catch (InvalidOperationException)
            {
                session = new PlayerSession
                {
                    sessionID = Utils.GenerateRandomHexString(32),
                    sessionKey = Utils.GenerateRandomHexString(16),
                    player = playerUUID.ContainsKey(uuid) ? playerUUID[uuid] : RegisterNewPlayer(uuid),
                };

                playerSessions.Add(session.sessionID, session);

                verb = "Created";
            }

            Utils.Log(
                $"{verb} session for player {session.player.id.code}\n\tSession  ID = {session.sessionID}\n\tSession Key = {session.sessionKey}");

            session.deviceInfo = new DeviceInfo
            {
                os = entity.GetRequestHeaderValue("X-APP-PLATFORM").Split(' ')[0] == "Android"
                    ? DeviceInfo.OS.Android
                    : DeviceInfo.OS.iOS,
                platformName = entity.GetRequestHeaderValue("X-APP-PLATFORM"),
                unityVersion = entity.GetRequestHeaderValue("X-Unity-Version"),
                appVersion = entity.GetRequestHeaderValue("X-APP-VERSION"),
                deviceName = entity.GetRequestHeaderValue("X-APP-DEVICE"),
                userAgent = entity.GetRequestHeaderValue("User-Agent"),
            };

            return session;
        }

        public static bool GetSessionFromCookie(this EntityBase entity, out PlayerSession session)
        {
            string cookie = entity.GetRequestHeaderValue("Cookie");
            var cookies = cookie.Split(';',
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Split('='))
                .ToDictionary(e => e[0], e => e.Length > 1 ? e[1] : "");

            if (cookies.ContainsKey("_session_id"))
            {
                session = playerSessions[cookies["_session_id"]];
                return true;
            }

            session = new PlayerSession();
            return false;
        }
    }
}