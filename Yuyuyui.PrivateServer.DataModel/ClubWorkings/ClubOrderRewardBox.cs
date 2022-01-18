using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ClubOrderRewardBox
    {
        public byte[]? Id { get; set; }
        public byte[]? Title { get; set; }
        public byte[]? ItemCategory { get; set; }
        public byte[]? ItemMasterId { get; set; }
        public byte[]? HasQuestion { get; set; }
    }
}
