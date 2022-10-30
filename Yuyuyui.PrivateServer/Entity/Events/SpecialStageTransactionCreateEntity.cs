using Yuyuyui.PrivateServer.Requests;

namespace Yuyuyui.PrivateServer.Events;

public class SpecialStageTransactionCreateEntity : BaseEntity<SpecialStageTransactionCreateEntity>
{
    public SpecialStageTransactionCreateEntity(
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
        var player = GetPlayerFromCookies();

        Utils.Log("Path parameters:");
        foreach (var pathParameter in pathParameters)
        {
            Utils.Log($"\t{pathParameter.Key} = {pathParameter.Value}");
        }
        
        long stageId = long.Parse(GetPathParameter("specialStageId"));
        
        BattleRequest request = Deserialize<BattleRequest>(requestBody)!;

        QuestTransaction.TransactionCreateData transactionCreateData = new QuestTransaction.TransactionCreateData()
        {
            supporting_deck_card_id = request.transaction.supporting_deck_card_id,
            using_deck_id = request.transaction.using_deck_id
        };
        
        QuestTransaction createdTransaction = QuestTransaction.Create(stageId, transactionCreateData);

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