﻿using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class FamiliarityLevel
    {
        public long Id { get; set; }
        public int Level { get; set; }
        public int? MaxExp { get; set; } = null;
        public int SupportPointBonus { get; set; }
        public float HitPointCoefficient { get; set; }
        public float AttackCoefficient { get; set; }
    }
}
