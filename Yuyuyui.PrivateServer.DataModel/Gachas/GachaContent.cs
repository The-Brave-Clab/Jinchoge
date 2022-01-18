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
        public string ContentType { get; set; } = "";
        public int Weight { get; set; }
        public long? SelectId { get; set; } = null;
        public int? SpecialWeightGroup { get; set; } = null;
        public string? Remarks { get; set; } = null; // All null
    }
}
