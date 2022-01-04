using System.Text;

namespace Yuyuyui.PrivateServer
{
    public class RegulationEntity : BaseEntity<RegulationEntity>
    {
        public const string REGULATION_CHECKED_FILE = "regulation.txt";
        public RegulationEntity(
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
            string regulationCheckedFile = Path.Combine(playerDataDir, REGULATION_CHECKED_FILE);

            if (requestBody.Length > 0)
            {
                Request request = Deserialize<Request>(requestBody)!;
                int checkVersion = request.regulation_version.current_version;
                Utils.Log($"Player agreed to regulation version {checkVersion}");
                File.WriteAllText(regulationCheckedFile, $"{checkVersion}");
            }

            if (!File.Exists(regulationCheckedFile))
            {
                File.WriteAllText(regulationCheckedFile, "0");
            }
            
            int checkedVersion = int.Parse(File.ReadAllText(regulationCheckedFile));
            
            Response responseObj = new()
            {
                regulation_version = new()
                {
                    current_version = 1,
                    checked_version = checkedVersion,
                    regulation_url = "http://download.cert"
                }
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Request
        {
            public RegulationVersion regulation_version { get; set; } = new();

            public class RegulationVersion
            {
                public int current_version { get; set; }
            }
        }

        public class Response
        {
            public RegulationVersion regulation_version { get; set; } = new();

            public class RegulationVersion
            {
                public int current_version { get; set; }
                public int checked_version { get; set; }
                public string regulation_url { get; set; } = "";
            }
        }
    }
}