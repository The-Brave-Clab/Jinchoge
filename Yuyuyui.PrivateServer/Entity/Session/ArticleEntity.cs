using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer;

public class ArticleEntity : BaseEntity<ArticleEntity>
{
    public ArticleEntity(
        Uri requestUri,
        string httpMethod,
        Dictionary<string, string> requestHeaders,
        byte[] requestBody,
        RouteConfig config)
        : base(requestUri, httpMethod, requestHeaders, requestBody, config)
    {
    }

    protected override Task ProcessRequest()
    {
        Response responseObj = new()
        {
            articles =
            [
                new()
                {
                    url = PrivateServer.ResourceProvider.urlProvider.Topics,
                    kind = 0,
                    label = "topics"
                },
                new()
                {
                    url = PrivateServer.ResourceProvider.urlProvider.Defects,
                    kind = 0,
                    label = "defect_topics"
                },
                new()
                {
                    url = PrivateServer.ResourceProvider.urlProvider.Inquiry,
                    kind = 0,
                    label = "inquiry"
                },
                new()
                {
                    url = PrivateServer.ResourceProvider.urlProvider.Terms,
                    kind = 0,
                    label = "terms"
                },
                new()
                {
                    url = PrivateServer.ResourceProvider.urlProvider.Helps,
                    kind = 0,
                    label = "helps"
                },
                new()
                {
                    url = PrivateServer.ResourceProvider.urlProvider.OfficialLinks,
                    kind = 0,
                    label = "official_links"
                }
            ]
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