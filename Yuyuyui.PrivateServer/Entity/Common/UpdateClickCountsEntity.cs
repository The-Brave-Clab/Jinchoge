using System.Net;
using System.Text;

namespace Yuyuyui.PrivateServer
{
    public class UpdateClickCountsEntity : BaseEntity<UpdateClickCountsEntity>
    {
        public UpdateClickCountsEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config,
            IPEndPoint localEndPoint)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config, localEndPoint)
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