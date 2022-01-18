using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class AutoClearTicket
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public long ImageId { get; set; }
        public int UsingTimesLimit { get; set; }
        public int? SecondsForUsingTimesResetTime { get; set; } = null;
    }
}
