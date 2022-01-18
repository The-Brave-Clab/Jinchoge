using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class AutoClearTicket
    {
        public byte[]? Id { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? Description { get; set; }
        public byte[]? ImageId { get; set; }
        public byte[]? UsingTimesLimit { get; set; }
        public byte[]? SecondsForUsingTimesResetTime { get; set; }
    }
}
