using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class BillingPointTransactionCreateEntity : BaseEntity<BillingPointTransactionCreateEntity>
    {
        public BillingPointTransactionCreateEntity(Uri requestUri, string httpMethod, Dictionary<string, string> requestHeaders, byte[] requestBody, RouteConfig config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config) { }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();
            long transactionId = long.Parse(Utils.GenerateRandomDigit(8));

            player.transactions.billingPointTransactions[transactionId] = new PlayerProfile.BillingPointTransaction
            {
                kind = GetTransactionKind()
            };
            player.Save();

            responseBody = Serialize(new Response { transaction = new Transaction { id = transactionId } });
            SetBasicResponseHeaders();
            return Task.CompletedTask;
        }

        private string GetTransactionKind()
        {
            if (RequestUri.AbsolutePath.Contains("weekday_stamina_recovery"))
                return "weekday_stamina_recovery";
            if (RequestUri.AbsolutePath.Contains("stamina_recovery"))
                return "stamina_recovery";
            if (RequestUri.AbsolutePath.Contains("enhancement_item_capacity"))
                return "enhancement_item_capacity";
            return "unknown";
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
