namespace Yuyuyui.PrivateServer
{
    public class ArticleEntity : BaseEntity<ArticleEntity>
    {
        public ArticleEntity(
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
                articles = new Response.Article[]
                {
                    new()
                    {
                        url = "http://download.cert",
                        kind = 0,
                        label = "topics"
                    },
                    new()
                    {
                        url = "http://download.cert",
                        kind = 0,
                        label = "defect_topics"
                    },
                    new()
                    {
                        url = "http://download.cert",
                        kind = 0,
                        label = "inquiry"
                    },
                    new()
                    {
                        url = "http://download.cert",
                        kind = 0,
                        label = "terms"
                    },
                    new()
                    {
                        url = "http://download.cert",
                        kind = 0,
                        label = "helps"
                    },
                    new()
                    {
                        url = "http://download.cert",
                        kind = 0,
                        label = "official_links"
                    }
                }
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IList<Article> articles { get; set; } = Array.Empty<Article>();

            public class Article
            {
                public string url { get; set; } = "";
                public int kind { get; set; }
                public string label { get; set; } = "";
            }
        }
    }
}