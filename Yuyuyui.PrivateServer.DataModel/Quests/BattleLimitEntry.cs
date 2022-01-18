using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class BattleLimitEntry
    {
        public long Id { get; set; }
        public string EnableId { get; set; } // "<long>,<long>,<long>" or "<long>...", character ids maybe?
    }
}
