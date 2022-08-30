using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class GachaLineup
    {
        public long Id { get; set; }
        public long GachaId { get; set; }
        public long GachaBoxId { get; set; }
        public int LotCount { get; set; }
        public int ConsumptionResourceId { get; set; }
        public int ConsumptionAmount { get; set; }
        public int? DailyLimit { get; set; } = null;
        public string? ButtonExtra { get; set; } = null;
        public string? ButtonTitle { get; set; } = null;
        public int Onetime { get; set; } // 01 boolean
        public int Sp { get; set; } // 01, Don't know what is this
        public int Pc { get; set; } // 01, Don't know what is this
        public int CountUpGacha { get; set; } // 01 boolean
        public int? CountDownGacha { get; set; } = null; // boolean, 1 for true, null for false
        public int FreeRareGacha { get; set; } // 01 boolean
    }
}
