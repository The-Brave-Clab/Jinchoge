﻿namespace Yuyuyui.PrivateServer
{
    public static class PrivateServer
    {
        public struct PlayerID
        {
            public string uuid { get; set; }
            public string code { get; set; }

            public static PlayerID FromString(string str)
            {
                var split = str.Split(",");
                return new PlayerID { uuid = split[0], code = split[1] };
            }

            public override string ToString()
            {
                return $"{uuid},{code}";
            }
        }

        public struct PlayerSession
        {
            public PlayerID player;
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

        private static Dictionary<string, PlayerID> playerUUID;
        private static Dictionary<string, PlayerID> playerCode;
        private static Dictionary<string, PlayerSession> playerSessions;

        private const string DATA_FOLDER = "Data";
        private const string PLAYER_DATA_FILE = "players.dat";

        static PrivateServer()
        {
            playerUUID = new Dictionary<string, PlayerID>();
            playerCode = new Dictionary<string, PlayerID>();
            playerSessions = new Dictionary<string, PlayerSession>();

            dataFolder = Utils.EnsureDirectory(DATA_FOLDER);
            var playerDataFile = Path.Combine(dataFolder, PLAYER_DATA_FILE);

            if (!File.Exists(playerDataFile))
            {
                File.AppendAllText(playerDataFile, null);
            }

            var players = File.ReadLines(playerDataFile);
            foreach (var s in players)
            {
                var player = PlayerID.FromString(s);
                playerUUID.Add(player.uuid, player);
                playerCode.Add(player.code, player);
            }
        }

        private static PlayerID RegisterNewPlayer(string uuid)
        {
            var player = new PlayerID { uuid = uuid, code = Utils.GenerateRandomPlayerCode() };
            playerUUID.Add(player.uuid, player);
            playerCode.Add(player.code, player);

            var playerDataFile = Path.Combine(dataFolder, PLAYER_DATA_FILE);
            File.AppendAllText(playerDataFile, $"{player}\n");

            Utils.Log($"Registered new player {player.code}");

            return player;
        }

        public static PlayerSession CreateSessionForPlayer(string uuid, EntityBase entity)
        {
            PlayerSession session;
            string verb = "";
            try
            {
                session = playerSessions.First(p => p.Value.player.uuid == uuid).Value;
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
                $"{verb} session for player {session.player.code}\n\tSession  ID = {session.sessionID}\n\tSession Key = {session.sessionKey}");

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

        public static string EnsurePlayerDataFolder(PlayerID playerID)
        {
            string dir = Path.Combine(DATA_FOLDER, $"{playerID.code}");
            return Utils.EnsureDirectory(dir);
        }
    }
}