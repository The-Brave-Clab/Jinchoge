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
            articles = PrivateServer.ResourceProvider.GetArticles()
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