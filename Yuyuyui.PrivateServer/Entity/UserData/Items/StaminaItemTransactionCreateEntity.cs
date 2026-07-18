using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class StaminaItemTransactionCreateEntity : BaseEntity<StaminaItemTransactionCreateEntity>
    {
        public StaminaItemTransactionCreateEntity(
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
            var player = GetPlayerFromCookies();
            long staminaItemId = long.Parse(GetPathParameter("stamina_item_id"));
            long transactionId = long.Parse(Utils.GenerateRandomDigit(8));

            player.transactions.staminaItemTransactions[transactionId] = new PlayerProfile.StaminaItemTransaction
            {
                stamina_item_id = staminaItemId
            };
            player.Save();

            responseBody = Serialize(new Response { transaction = new Transaction { id = transactionId } });
            SetBasicResponseHeaders();
            return Task.CompletedTask;
        }

        public class Response
        {
            public Transaction transaction { get; set; } = new();
        }

        public class Transaction
        {
            public long id { get; set; }
        }
    }
}
