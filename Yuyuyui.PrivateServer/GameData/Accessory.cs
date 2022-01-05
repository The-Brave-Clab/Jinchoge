namespace Yuyuyui.PrivateServer
{
    public class Accessory : UserDataBase<Accessory>
    {
        public long id { get; set; }
        public int master_id { get; set; } // from master_data
        public int level { get; set; }
        public int cost { get; set; }
        public int hit_point { get; set; }
        public int attack { get; set; }
        public int money { get; set; }
        public int quantity { get; set; }
        public int next_quantity { get; set; }

        public static Accessory DefaultAccessory()
        {
            long new_id = long.Parse(Utils.GenerateRandomDigit(8));
            while (Exists($"{new_id}"))
            {
                new_id = long.Parse(Utils.GenerateRandomDigit(8));
            }
            return new Accessory
            {
                id = new_id,
                master_id = 500001, // Gyuuki
                level = 1,
                cost = 11,
                hit_point = 2230,
                attack = 0,
                money = 1000,
                quantity = 0,
                next_quantity = 1,
            };
        }

        protected override string Identifier => $"{id}";
    }
}

