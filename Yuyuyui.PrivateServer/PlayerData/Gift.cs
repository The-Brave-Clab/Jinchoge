using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class Gift : BasePlayerData<Gift, long>
    {
        public override long Identifier => id;

        public long id { get; set; }
        public string title { get; set; } = ""; // items.db/gifts
        public string name { get; set; } = ""; // items.db/gifts
        public long reception_at { get; set; } // unixtime
        public long received_at { get; set; } // unixtime, 0 if not applicable, player accepted the gift
        public long receivable_at { get; set; } // unixtime, 0 if not applicable, player haven't accepted the gift
        public int quantity { get; set; }
        public int item_category_id { get; set; }
        public int item_id { get; set; }

        public async Task ReceivedByPlayer(PlayerProfile player)
        {
            received_at = 0;
            receivable_at = Utils.CurrentUnixTime();
            await Save();
            
            player.receivedGifts.Add(id);
            await player.Save();
        }

        public async Task AcceptedByPlayer(PlayerProfile player)
        {
            received_at = Utils.CurrentUnixTime();
            receivable_at = 0;
            await Save();

            player.receivedGifts.Remove(id);
            player.acceptedGifts.Add(id);
            await player.Save();
        }
    }
}