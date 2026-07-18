using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class ClubWorkingForceCompleteTransactionCreateEntity : BaseEntity<ClubWorkingForceCompleteTransactionCreateEntity>
    {
        public ClubWorkingForceCompleteTransactionCreateEntity(
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
                transaction = new()
                {
                    id = long.Parse(Utils.GenerateRandomDigit(8))
                }
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public Transaction transaction { get; set; } = new();

            public class Transaction
            {
                public long id { get; set; }
            }
        }
    }
}
