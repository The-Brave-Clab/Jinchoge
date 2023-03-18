using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class BattleContinueInfoEntity : BaseEntity<BattleContinueInfoEntity>
    {
        public BattleContinueInfoEntity(
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
            long transactionId = long.Parse(GetPathParameter("transaction_id"));
            
            // ignore request body

            var responseObj = new Response
            {
                // TODO: check for "story" or "special"
                battle_continue = QuestTransaction.Exists(transactionId)
            };
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public bool battle_continue { get; set; } = true; // true for continue, false for error
        }
    }
}