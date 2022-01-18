using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class GachaTicket
    {
        public byte[]? Id { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? GachaKind { get; set; }
        public byte[]? ConsumptionResourceId { get; set; }
        public byte[]? ImageId { get; set; }
        public byte[]? GachaId { get; set; }
    }
}
