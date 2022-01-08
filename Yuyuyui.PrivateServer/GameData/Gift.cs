namespace Yuyuyui.PrivateServer
{
    public class Gift : BasePlayerData<Gift, long>
    {
        protected override long Identifier => id;

        public long id { get; set; }
        public string title { get; set; } = "";
        public string name { get; set; } = "";
        public long reception_at { get; set; } // unixtime
        public long received_at { get; set; } // unixtime, 0 if not applicable, player accepted the gift
        public long receivable_at { get; set; } // unixtime, 0 if not applicable, player haven't accepted the gift
        public int quantity { get; set; }
        public int item_category_id { get; set; }
        public int item_id { get; set; }

        public void ReceivedByPlayer(PlayerProfile player)
        {
            received_at = 0;
            receivable_at = Utils.CurrentUnixTime();
            Save();
            
            player.receivedGifts.Add(id);
            player.Save();
        }

        public void AcceptedByPlayer(PlayerProfile player)
        {
            received_at = Utils.CurrentUnixTime();
            receivable_at = 0;
            Save();

            player.receivedGifts.Remove(id);
            player.acceptedGifts.Add(id);
            player.Save();
        }
    }
}