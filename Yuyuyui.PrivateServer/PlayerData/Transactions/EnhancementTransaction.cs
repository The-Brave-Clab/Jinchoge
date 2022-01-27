namespace Yuyuyui.PrivateServer
{
    public class EnhancementTransaction : BasePlayerData<EnhancementTransaction, long>
    {
        public long id { get; set; }
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

        public static EnhancementTransaction Create(TransactionCreateData createData)
        {
            EnhancementTransaction transaction = new()
            {
                id = GetID(),
                createdWith = createData
            };
            
            transaction.Save();
            
            return transaction;
        }
        
        // Used by json
        public class TransactionCreateData
        {
            public long card_id { get; set; }
            public EnhancementItemUsedData enhancement_item { get; set; } = new();

            public class EnhancementItemUsedData
            {
                public long id { get; set; }
                public int quantity { get; set; }
            }
        }
    }
}