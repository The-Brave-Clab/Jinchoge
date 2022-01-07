using System.Net;

namespace Yuyuyui.PrivateServer
{
    public class RequirementVersionEntity : BaseEntity<RequirementVersionEntity>
    {
        public RequirementVersionEntity(
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
            Utils.LogError(localEndPoint.Address);
            Response responseObj = new()
            {
                requirement_version = new()
                {
                    version = "3.15.0",
                    need_update = false,
                    review = false,
                    api_server = $"http://{localEndPoint.Address}:44461", // TODO
                    enable_cooperation = false
                }
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public RequirementVersion requirement_version { get; set; } = new();

            public class RequirementVersion
            {
                public string version { get; set; } = "";
                public bool need_update { get; set; }
                public bool review { get; set; }
                public string api_server { get; set; } = "";
                public bool enable_cooperation { get; set; }
            }
        }
    }
}