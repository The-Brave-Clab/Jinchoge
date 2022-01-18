using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class GachaContent
    {
        public long Id { get; set; }
        public long GachaBoxId { get; set; }
        public int Category { get; set; }
        public long ContentId { get; set; }
        public string ContentType { get; set; }
        public int Weight { get; set; }
        public long? SelectId { get; set; }
        public int? SpecialWeightGroup { get; set; }
        public string? Remarks { get; set; } // All null
    }
}
