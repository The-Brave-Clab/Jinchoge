using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ClubOrderRewardBox
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public int ItemCategory { get; set; }
        public long? ItemMasterId { get; set; }
        public int HasQuestion { get; set; } // 01 boolean
    }
}
