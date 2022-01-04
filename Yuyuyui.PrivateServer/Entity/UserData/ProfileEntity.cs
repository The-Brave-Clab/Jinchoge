using System.Text;

namespace Yuyuyui.PrivateServer
{
    public class ProfileEntity : BaseEntity<ProfileEntity>
    {
        public const string PROFILE_FILE = "profile.json";
        public ProfileEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            bool isSession = this.GetSessionFromCookie(out var playerSession);
            if (!isSession)
            {
                throw new Exception("Session not found!");
            }
            
            string playerDataDir = PrivateServer.EnsurePlayerDataFolder(playerSession.player);
            string profileFile = Path.Combine(playerDataDir, PROFILE_FILE);

            if (requestBody.Length > 0)
            {
                // Doing a deserialization and then a serialization will escape the unicode characters
                RequestResponse request = Deserialize<RequestResponse>(requestBody)!;
                byte[] body = Serialize(request);
                Console.WriteLine(Encoding.UTF8.GetString(body));
                File.WriteAllBytes(profileFile, body);
            }

            if (!File.Exists(profileFile))
            {
                File.WriteAllText(profileFile, 
                    "{\"profile\":{\"nickname\":\"NONE\",\"comment\":\"よく寝て、よく食べる\"}}");
            }
            
            responseBody = File.ReadAllBytes(profileFile);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class RequestResponse
        {
            public Profile profile { get; set; } = new();

            public class Profile
            {
                public string nickname { get; set; } = "";
                public string comment { get; set; } = "";
            }
        }
    }
}