namespace Yuyuyui.PrivateServer
{
    public class QuestTransaction : BasePlayerData<QuestTransaction, long>
    {
        public long id { get; set; }
        public long stageId { get; set; }
        public TransactionCreateData createdWith { get; set; } = new();

        protected override long Identifier => id;
        
        private static long GetID()
        {
            long new_id = long.Parse(Utils.GenerateRandomDigit(8));
            while (Exists(new_id))
            {
                new_id = long.Parse(Utils.GenerateRandomDigit(8));
            }

            return new_id;
        }

        public static QuestTransaction Create(long stageId, TransactionCreateData createData)
        {
            QuestTransaction transaction = new()
            {
                id = GetID(),
                stageId = stageId,
                createdWith = createData
            };
            
            transaction.Save();
            
            return transaction;
        }
        
        
        public class TransactionCreateData
        {
            public long? using_deck_id { get; set; } = null;
            public long? supporting_deck_card_id { get; set; } = null;
        }
    }
}