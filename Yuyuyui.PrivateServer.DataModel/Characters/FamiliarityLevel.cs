using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class FamiliarityLevel
    {
        public byte[]? Id { get; set; }
        public byte[]? Level { get; set; }
        public byte[]? MaxExp { get; set; }
        public byte[]? SupportPointBonus { get; set; }
        public byte[]? HitPointCoefficient { get; set; }
        public byte[]? AttackCoefficient { get; set; }
    }
}
