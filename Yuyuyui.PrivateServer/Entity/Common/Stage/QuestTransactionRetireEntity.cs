using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class QuestTransactionRetireEntity : BaseEntity<QuestTransactionRetireEntity>
    {
        public QuestTransactionRetireEntity(
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
            long transactionId = long.Parse(GetPathParameter("transaction_id"));
            
            QuestTransaction transaction = QuestTransaction.Load(transactionId);
            // Validate?

            player.transactions.questTransactions.Remove(transaction.stageId);
            player.Save();
            transaction.Delete();

            responseBody = Encoding.UTF8.GetBytes("{}");
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