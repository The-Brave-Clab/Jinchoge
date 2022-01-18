using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class BingoReward
    {
        public byte[]? Id { get; set; }
        public byte[]? ContentId { get; set; }
        public byte[]? ContentType { get; set; }
        public byte[]? Quantity { get; set; }
        public byte[]? Title { get; set; }
    }
}
