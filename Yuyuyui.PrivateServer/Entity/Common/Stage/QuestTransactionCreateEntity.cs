using System;
using System.Collections.Generic;
using System.Text;
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

            long stageId = long.Parse(GetPathParameter("stage_id"));

            QuestTransaction.TransactionCreateData transactionCreateData;

            // hardcoded string from client, which I believe is a bug
            if (Encoding.UTF8.GetString(requestBody) == "{\"supporting_deck_card_id\":null, \"using_deck_id\":null}")
            {
                transactionCreateData = Deserialize<QuestTransaction.TransactionCreateData>(requestBody)!;
            }
            else
            {
                var requestObj = Deserialize<Request>(requestBody)!;
                transactionCreateData = new()
                {
                    using_deck_id = requestObj.transaction.using_deck_id,
                    // TODO: This is for now ignored since we don't really have a working friend system
                    supporting_deck_card_id = null //requestObj.transaction.supporting_deck_card_id
                };
            }

            // Delete duplicate/unfinished quests
            // TODO: we do this here because we have not yet implemented continue feature.
            if (player.transactions.questTransactions.ContainsKey(stageId))
            {
                var existedTransaction = QuestTransaction.Load(player.transactions.questTransactions[stageId]);
                existedTransaction.Delete();
                player.transactions.questTransactions.Remove(stageId);
            }
            var createdTransaction = QuestTransaction.Create(stageId, transactionCreateData);
            player.transactions.questTransactions.Add(stageId, createdTransaction.id);
            player.Save();

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
        
        public class Request
        {
            public int stage_id { get; set; }
            public Transaction transaction { get; set; } = new();

            public class Transaction
            {
                public long id { get; set; }
                public long? stage_id { get; set; } = null;
                public long? using_deck_id { get; set; } = null;
                public long? supporting_deck_card_id { get; set; } = null;
                public bool no_friend { get; set; }
            }
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