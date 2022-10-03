namespace Yuyuyui.PrivateServer
{
    public class EnhancementResultTransactionCreateEntity : BaseEntity<EnhancementResultTransactionCreateEntity>
    {
        public EnhancementResultTransactionCreateEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            //var player = GetPlayerFromCookies();
            
            // path parameters are ignored
            Utils.Log("Path parameters:");
            foreach (var pathParameter in pathParameters)
            {
                Utils.Log($"\t{pathParameter.Key} = {pathParameter.Value}");
            }

            EnhancementTransaction.TransactionCreateData request =
                Deserialize<EnhancementTransaction.TransactionCreateData>(requestBody)!;

            EnhancementTransaction createdTransaction = EnhancementTransaction.Create(request);

            Response responseObj = new()
            {
                transaction = new()
                {
                    id = createdTransaction.id,
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