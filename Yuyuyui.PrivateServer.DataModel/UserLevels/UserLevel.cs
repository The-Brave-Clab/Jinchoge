using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class UserLevel
    {
        public long Id { get; set; }
        public int Level { get; set; } // though the value is the same with id, we do int here for easier usage
        public int MaxStamina { get; set; }
        public long MaxExp { get; set; }
        public int MaxFellow { get; set; }
        public int EnhancementItemCapacity { get; set; }
    }
}
