using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class TimeLimitedBuff
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Kind { get; set; }
        public float Rate { get; set; }
    }
}
