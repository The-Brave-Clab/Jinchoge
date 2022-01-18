using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class TimeLimitedBuff
    {
        public byte[]? Id { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? Kind { get; set; }
        public byte[]? Rate { get; set; }
    }
}
