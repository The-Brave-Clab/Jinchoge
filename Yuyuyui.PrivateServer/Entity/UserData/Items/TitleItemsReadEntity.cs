using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class TitleItemsReadEntity : BaseEntity<TitleItemsReadEntity>
    {
        public TitleItemsReadEntity(
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

            //Request requestObj = Deserialize<Request>(requestBody)!;

            responseBody = Encoding.UTF8.GetBytes("{}");

            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Request
        {
            public int type { get; set; }
            public int character_id { get; set; }
        }
    }
}