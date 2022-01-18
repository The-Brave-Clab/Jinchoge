using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class UserLevel
    {
        public byte[]? Id { get; set; }
        public byte[]? Level { get; set; }
        public byte[]? MaxStamina { get; set; }
        public byte[]? MaxExp { get; set; }
        public byte[]? MaxFellow { get; set; }
        public byte[]? EnhancementItemCapacity { get; set; }
    }
}
