namespace Yuyuyui.PrivateServer
{
    public class RegulationEntity : BaseEntity<RegulationEntity>
    {
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
            Response responseObj = new()
            {
                regulation_version = new()
                {
                    current_version = 1,
                    checked_version = 1,
                    regulation_url = "http://download.cert"
                }
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
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