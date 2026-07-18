using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class GachaTransactionCreateEntity : BaseEntity<GachaTransactionCreateEntity>
    {
        public GachaTransactionCreateEntity(
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
            long gachaId = long.Parse(GetPathParameter("gacha_id"));
            long lineupId = long.Parse(GetPathParameter("lineup_id"));
            long transactionId = long.Parse(Utils.GenerateRandomDigit(8));

            player.transactions.gachaTransactions[transactionId] = new PlayerProfile.GachaTransaction
            {
                gacha_id = gachaId,
                lineup_id = lineupId
            };
            player.Save();

            Response responseObj = new()
            {
                transaction = new()
                {
                    id = transactionId
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
