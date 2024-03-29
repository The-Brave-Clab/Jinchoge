﻿namespace Yuyuyui.PrivateServer
{
    public class ClubOrder : BasePlayerData<ClubOrder, long>
    {
        public long id { get; set; } // 7 digits
        public int master_id { get; set; } // from master_data
        public int quantity { get; set; }

        public class RewardBox
        {
            public long id { get; set; } // 8 digits
            public string title { get; set; } = "";
        }

        protected override long Identifier => id;
    }
}