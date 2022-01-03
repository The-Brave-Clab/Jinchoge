namespace Yuyuyui.PrivateServer
{
    public class RequirementVersionEntity : BaseEntity<RequirementVersionEntity>
    {
        public RequirementVersionEntity(
            Uri requestUri,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config)
            : base(requestUri, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            Response responseObj = new()
            {
                requirement_version = new()
                {
                    version = "3.14.0",
                    need_update = false,
                    review = false,
                    api_server = "https://app.yuyuyui.jp",
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