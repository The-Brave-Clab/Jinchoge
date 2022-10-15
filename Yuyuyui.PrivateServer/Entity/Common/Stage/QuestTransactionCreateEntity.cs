using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class QuestTransactionCreateEntity : BaseEntity<QuestTransactionCreateEntity>
    {
        public QuestTransactionCreateEntity(
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
            
            Utils.Log("Path parameters:");
            foreach (var pathParameter in pathParameters)
            {
                Utils.Log($"\t{pathParameter.Key} = {pathParameter.Value}");
            }

            long stageId = long.Parse(GetPathParameter("stage_id"));

            QuestTransaction.TransactionCreateData request =
                Deserialize<QuestTransaction.TransactionCreateData>(requestBody)!;

            QuestTransaction createdTransaction = QuestTransaction.Create(stageId, request);

            Response responseObj = new()
            {
                transaction = new()
                {
                    id = createdTransaction.id,
                    stage_id = createdTransaction.stageId
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
                public long stage_id { get; set; }
            }
        }
    }
}