using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class GachaBox
    {
        public byte[]? Id { get; set; }
        public byte[]? Description { get; set; }
        public byte[]? Category1Weight { get; set; }
        public byte[]? Category2Weight { get; set; }
        public byte[]? Category3Weight { get; set; }
        public byte[]? Category4Weight { get; set; }
    }
}
