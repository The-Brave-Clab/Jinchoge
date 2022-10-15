using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class UpdateClickCountsEntity : BaseEntity<UpdateClickCountsEntity>
    {
        public UpdateClickCountsEntity(
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
            Utils.LogError("Not documented! Stubbed for now.");

            //Request requestObj = Deserialize<Request>(requestBody)!;

            responseBody = Encoding.UTF8.GetBytes("{}");
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Request
        {
            public int no { get; set; }
            public IDictionary<string, int> param = new Dictionary<string, int>();
            
            // {
            //     "no": 2,
            //     "param": {
            //         "image_id": 1531
            //     }
            // }
        }
    }
}