namespace Yuyuyui.PrivateServer
{
    public class Accessory : BasePlayerData<Accessory, long>
    {
        public long id { get; set; }
        public int master_id { get; set; } // from master_data
        public int level { get; set; }
        public int quantity { get; set; }

        public static Accessory DefaultAccessory()
        {
            long new_id = long.Parse(Utils.GenerateRandomDigit(9));
            while (Exists(new_id))
            {
                new_id = long.Parse(Utils.GenerateRandomDigit(9));
            }
            return new Accessory
            {
                id = new_id,
                master_id = 500001, // Gyuuki, consider reading the data from database
                level = 1,
                quantity = 0,
            };
        }

        protected override long Identifier => id;
    }
}

