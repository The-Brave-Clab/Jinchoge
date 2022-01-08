namespace Yuyuyui.PrivateServer
{
    public class Item : BasePlayerData<Item, long>
    {
        public long id { get; set; } // 8 digits
        public int master_id { get; set; } // from master_data
        public int quantity { get; set; }

        private static long GetID()
        {
            long new_id = long.Parse(Utils.GenerateRandomDigit(7));
            while (Exists(new_id))
            {
                new_id = long.Parse(Utils.GenerateRandomDigit(7));
            }

            return new_id;
        }

        protected override long Identifier => id;
    }
}