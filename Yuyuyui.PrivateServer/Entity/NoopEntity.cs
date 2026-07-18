using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class NoopEntity : BaseEntity<NoopEntity>
    {
        public NoopEntity(
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
            responseBody = Serialize(new Response());
            SetBasicResponseHeaders();
            return Task.CompletedTask;
        }

        public class Response
        {
            public bool accepted { get; set; } = true;
            public bool completed { get; set; } = true;
        }
    }
}
