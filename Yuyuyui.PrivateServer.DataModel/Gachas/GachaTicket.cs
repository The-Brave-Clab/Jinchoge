using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class GachaTicket
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public int GachaKind { get; set; }
        public int ConsumptionResourceId { get; set; }
        public long ImageId { get; set; }
        public long GachaId { get; set; }
    }
}
