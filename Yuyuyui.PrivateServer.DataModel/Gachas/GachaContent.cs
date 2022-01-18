using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class GachaContent
    {
        public byte[]? Id { get; set; }
        public byte[]? GachaBoxId { get; set; }
        public byte[]? Category { get; set; }
        public byte[]? ContentId { get; set; }
        public byte[]? ContentType { get; set; }
        public byte[]? Weight { get; set; }
        public byte[]? SelectId { get; set; }
        public byte[]? SpecialWeightGroup { get; set; }
        public byte[]? Remarks { get; set; }
    }
}
