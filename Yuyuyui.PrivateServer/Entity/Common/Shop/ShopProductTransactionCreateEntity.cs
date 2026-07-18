using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class ShopProductTransactionCreateEntity : BaseEntity<ShopProductTransactionCreateEntity>
    {
        public ShopProductTransactionCreateEntity(
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
            string shopId = GetPathParameter("shop_id");
            long productId = long.Parse(GetPathParameter("product_id"));
            long transactionId = long.Parse(Utils.GenerateRandomDigit(8));

            player.transactions.shopProductTransactions[transactionId] = new PlayerProfile.ShopProductTransaction
            {
                shop_id = shopId,
                product_id = productId
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
