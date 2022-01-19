namespace Yuyuyui.PrivateServer
{
    public class RequirementVersionEntity : BaseEntity<RequirementVersionEntity>
    {
        public RequirementVersionEntity(
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
            Response responseObj = new()
            {
                requirement_version = new()
                {
                    version = PrivateServer.YUYUYUI_APP_VERSION,
                    need_update = false,
                    review = false,
                    api_server = $"http://{PrivateServer.PRIVATE_LOCAL_API_SERVER}",
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