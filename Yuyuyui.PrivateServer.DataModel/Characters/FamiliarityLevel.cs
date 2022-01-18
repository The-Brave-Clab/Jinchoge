using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class FamiliarityLevel
    {
        public long Id { get; set; }
        public int Level { get; set; }
        public long? MaxExp { get; set; }
        public int SupportPointBonus { get; set; }
        public float HitPointCoefficient { get; set; }
        public float AttackCoefficient { get; set; }
    }
}
