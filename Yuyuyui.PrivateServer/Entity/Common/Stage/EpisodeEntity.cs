namespace Yuyuyui.PrivateServer
{
    public class EpisodeEntity : BaseEntity<EpisodeEntity>
    {
        public EpisodeEntity(
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
            var player = GetPlayerFromCookies();
            
            Utils.Log("Path parameters:");
            foreach (var pathParameter in pathParameters)
            {
                Utils.Log($"\t{pathParameter.Key} = {pathParameter.Value}");
            }

            Utils.LogWarning("Stub API, only returns the one that is required by tutorial for now");

            Response responseObj = new()
            {
                episodes = new Dictionary<int, Response.Episode>
                {
                    {
                        1001, new()
                        {
                            id = 1001,
                            master_id = 1001,
                            finish = false,
                            detail_url = "https://article.yuyuyui.jp/article/episodes/1001"
                        }
                    }
                }
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IDictionary<int, Episode> episodes { get; set; } = new Dictionary<int, Episode>();

            public class Episode
            {
                public long id { get; set; }
                public long master_id { get; set; } // Don't know the difference
                public bool finish { get; set; }
                public string detail_url { get; set; } = "";
            }
        }
    }
}