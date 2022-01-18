using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class StackPointEventReward
    {
        public byte[]? Id { get; set; }
        public byte[]? StackPointEventId { get; set; }
        public byte[]? NeedPoint { get; set; }
        public byte[]? GiftId { get; set; }
        public byte[]? ContentId { get; set; }
        public byte[]? ContentType { get; set; }
        public byte[]? Quantity { get; set; }
        public byte[]? PickUp { get; set; }
    }
}
