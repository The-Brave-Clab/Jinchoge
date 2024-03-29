using System.Linq;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class Accessory : BasePlayerData<Accessory, long>
    {
        public long id { get; set; }
        public long master_id { get; set; } // from master_data
        public int level { get; set; }
        public int quantity { get; set; }

        public static long GetID()
        {
            long new_id = long.Parse(Utils.GenerateRandomDigit(9));
            while (Exists(new_id))
            {
                new_id = long.Parse(Utils.GenerateRandomDigit(9));
            }

            return new_id;
        }
        
        public static Accessory DefaultAccessory()
        {
            return NewAccessoryByMasterId(500001); // Gyuuki, consider reading the data from database
        }
        
        public static Accessory NewAccessoryByMasterId(long masterId)
        {
            return new Accessory
            {
                id = GetID(),
                master_id = masterId,
                level = 1,
                quantity = 0,
            };
        }

        public DataModel.Accessory MasterData(AccessoriesContext accessoriesDb)
        {
            return accessoriesDb.Accessories.First(c => c.Id == master_id);
        }

        protected override long Identifier => id;
    }
}

