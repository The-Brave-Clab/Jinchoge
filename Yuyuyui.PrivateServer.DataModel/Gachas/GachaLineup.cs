using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class GachaLineup
    {
        public byte[]? Id { get; set; }
        public byte[]? GachaId { get; set; }
        public byte[]? GachaBoxId { get; set; }
        public byte[]? LotCount { get; set; }
        public byte[]? ConsumptionResourceId { get; set; }
        public byte[]? ConsumptionAmount { get; set; }
        public byte[]? DailyLimit { get; set; }
        public byte[]? ButtonExtra { get; set; }
        public byte[]? ButtonTitle { get; set; }
        public byte[]? Onetime { get; set; }
        public byte[]? Sp { get; set; }
        public byte[]? Pc { get; set; }
        public byte[]? CountUpGacha { get; set; }
        public byte[]? CountDownGacha { get; set; }
        public byte[]? FreeRareGacha { get; set; }
    }
}
