using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class TitleItemsCheckEntity : BaseEntity<TitleItemsCheckEntity>
    {
        public TitleItemsCheckEntity(
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
            //var player = GetPlayerFromCookies();
            
            // Utils.LogWarning("Stub API");

            // Request seems to be always "{}"
            //Request requestObj = Deserialize<Request>(requestBody)!;

            responseBody = Serialize(new Response());

            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IList<int> title_items { get; set; } = new List<int>(); // seems to be always empty? type not confirmed
        }
    }
}