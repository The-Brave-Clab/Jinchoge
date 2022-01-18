using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class AccessoryLevel
    {
        public byte[]? Id { get; set; }
        public byte[]? Rarity { get; set; }
        public byte[]? Level { get; set; }
        public byte[]? NeedAmount { get; set; }
        public byte[]? Money { get; set; }
        public byte[]? BraveCoin { get; set; }
    }
}
