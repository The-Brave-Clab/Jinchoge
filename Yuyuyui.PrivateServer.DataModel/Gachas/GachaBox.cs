using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class GachaBox
    {
        public long Id { get; set; }
        public string Description { get; set; } = "";
        public int Category1Weight { get; set; }
        public int Category2Weight { get; set; }
        public int Category3Weight { get; set; }
        public int Category4Weight { get; set; }
    }
}
