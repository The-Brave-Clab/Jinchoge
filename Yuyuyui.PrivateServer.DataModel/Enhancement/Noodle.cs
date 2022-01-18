using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Noodle
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int ExpCoefficient { get; set; }
        public long ImageId { get; set; }
    }
}
