using System.Net;
using System.Text;

namespace Yuyuyui.PrivateServer
{
    public class RegulationEntity : BaseEntity<RegulationEntity>
    {
        public RegulationEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config,
            IPEndPoint localEndPoint)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config, localEndPoint)
        {
        }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();

            if (requestBody.Length > 0)
            {
                Request request = Deserialize<Request>(requestBody)!;
                int checkVersion = request.regulation_version.current_version;
                Utils.Log($"Player agreed to regulation version {checkVersion}");
                player.data.regulationVersion = checkVersion;
                player.Save();
            }
            
            Response responseObj = new()
            {
                regulation_version = new()
                {
                    current_version = 1,
                    checked_version = player.data.regulationVersion,
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