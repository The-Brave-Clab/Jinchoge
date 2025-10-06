using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class Item : BasePlayerData<Item, long>
    {
        public long id { get; set; }
        public long master_id { get; set; } // from master_data
        public int quantity { get; set; }

        public static async Task<long> GetID()
        {
            long new_id = long.Parse(Utils.GenerateRandomDigit(9));
            while (await Exists(new_id))
            {
                new_id = long.Parse(Utils.GenerateRandomDigit(9));
            }

            return new_id;
        }

        protected override long Identifier => id;
    }
}