namespace Yuyuyui.PrivateServer
{
    public class TitleItem : BasePlayerData<TitleItem, long>
    {
        public long id { get; set; }
        public long master_id { get; set; } // from master_data
        public bool verified { get; set; }
        public bool is_grayout { get; set; }
        public bool is_max_evo { get; set; }

        public static long GetID()
        {
            long new_id = long.Parse(Utils.GenerateRandomDigit(9));
            while (Exists(new_id))
            {
                new_id = long.Parse(Utils.GenerateRandomDigit(9));
            }

            return new_id;
        }

        protected override long Identifier => id;
    }
}