using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class BattleContinueEntity : BaseEntity<BattleContinueEntity>
    {
        public BattleContinueEntity(
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
            var requestObj = Deserialize<Request>(requestBody)!;

            bool isStory = requestObj.type == "story"; // "special" otherwise

            var responseObj = new Response
            {
                transaction = new()
                {
                    id = requestObj.stage_transaction_id
                }
            };
            
            // TODO: cost player blessing here?
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }
        
        public class Request
        {
            public string type { get; set; } = "";
            public long stage_transaction_id { get; set; }
        }

        public class Response
        {
            public Transaction transaction { get; set; } = new();

            public class Transaction
            {
                public long id { get; set; } // This should be it's own type, but we use id from QuestTransaction instead for now
            }
        }
    }
}