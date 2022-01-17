namespace Yuyuyui.PrivateServer
{
    public class ClubWorkingSlot : BasePlayerData<ClubWorkingSlot, long>
    {
        public long id { get; set; }
        public bool available { get; set; }
        public long? club_working_id { get; set; } = null;
        public long? primary_user_card_id { get; set; } = null;
        public long? secondary_user_card_id { get; set; } = null;
        public int? club_order_master_id { get; set; } = null;
        public long? finishment_time { get; set; } = null; // unixtime

        public static ClubWorkingSlot NewEmptySlot()
        {
            long new_id = long.Parse(Utils.GenerateRandomDigit(9));
            while (Exists(new_id))
            {
                new_id = long.Parse(Utils.GenerateRandomDigit(9));
            }
            return new ClubWorkingSlot
            {
                id = new_id,
                available = true,
                club_working_id = null,
                primary_user_card_id = null,
                secondary_user_card_id = null,
                club_order_master_id = null,
                finishment_time = null
            };
        }

        protected override long Identifier => id;
    }
}