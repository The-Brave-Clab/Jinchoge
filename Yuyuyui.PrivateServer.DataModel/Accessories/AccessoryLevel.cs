using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class AccessoryLevel
    {
        public long Id { get; set; }
        public int Rarity { get; set; }
        public int Level { get; set; }
        public int NeedAmount { get; set; }
        public long Money { get; set; }
        public int BraveCoin { get; set; }
    }
}
