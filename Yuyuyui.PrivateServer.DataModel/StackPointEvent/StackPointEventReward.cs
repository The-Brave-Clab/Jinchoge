using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class StackPointEventReward
    {
        public long Id { get; set; }
        public long StackPointEventId { get; set; }
        public long NeedPoint { get; set; }
        public long GiftId { get; set; }
        public long ContentId { get; set; }
        public string ContentType { get; set; } = "";
        public long Quantity { get; set; }
        public int? PickUp { get; set; } = null; // boolean, 1 for true, null for false
    }
}
