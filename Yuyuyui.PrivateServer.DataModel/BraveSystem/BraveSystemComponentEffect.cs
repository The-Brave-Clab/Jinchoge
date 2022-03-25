using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class BraveSystemComponentEffect
    {
        public long Id { get; set; }
        public long BraveSystemComponentId { get; set; }
        public int Level { get; set; }
        public int Effect { get; set; }
    }
}
