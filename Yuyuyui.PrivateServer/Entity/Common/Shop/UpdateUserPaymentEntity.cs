using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class UpdateUserPaymentEntity : BaseEntity<UpdateUserPaymentEntity>
    {
        public UpdateUserPaymentEntity(Uri requestUri, string httpMethod, Dictionary<string, string> requestHeaders, byte[] requestBody, RouteConfig config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config) { }

        protected override Task ProcessRequest()
        {
            responseBody = Serialize(new Response { accepted = true });
            SetBasicResponseHeaders();
            return Task.CompletedTask;
        }

        public class Response
        {
            public bool accepted { get; set; }
        }
    }
}
