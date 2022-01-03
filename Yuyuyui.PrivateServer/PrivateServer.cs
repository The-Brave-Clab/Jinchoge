namespace Yuyuyui.PrivateServer
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
                return new PlayerID {uuid = split[0], code = split[1]};
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
            var player = new PlayerID {uuid = uuid, code = Utils.GenerateRandomPlayerCode()};
            playerUUID.Add(player.uuid, player);
            playerCode.Add(player.code, player);
            
            var playerDataFile = Path.Combine(dataFolder, PLAYER_DATA_FILE);
            File.AppendAllText(playerDataFile, $"{player}\n");
            
            Console.WriteLine($"Registered new player: {player}");
            
            return player;
        }

        public static PlayerSession CreateSessionForPlayer(string uuid)
        {
            try
            {
                PlayerSession foundSession = playerSessions.First(p => p.Value.player.uuid == uuid).Value;
                Console.WriteLine($"Found session for player {foundSession.player}: Session ID = {foundSession.sessionID}, Session Key = {foundSession.sessionKey}");
                return foundSession;
            }
            catch (InvalidOperationException)
            {
                var session = new PlayerSession
                {
                    sessionID = Utils.GenerateRandomHexString(32),
                    sessionKey = Utils.GenerateRandomHexString(16),
                    player = playerUUID.ContainsKey(uuid) ? playerUUID[uuid] : RegisterNewPlayer(uuid)
                };

                playerSessions.Add(session.sessionID, session);
                
                Console.WriteLine($"Created session for player {session.player}: Session ID = {session.sessionID}, Session Key = {session.sessionKey}");
            
                return session;
            }
        }

        public static bool GetSessionFromCookie(this EntityBase entity, out PlayerSession session)
        {
            string cookie = entity.GetRequestHeaderValue("Cookie");
            var cookies = cookie.Split(';',
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Split('='))
                .ToDictionary(e => e[0], e => e[1]);

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