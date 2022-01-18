using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class EvolutionRecipe
    {
        public long Id { get; set; }
        public long Cost { get; set; }
        public long Resource1Id { get; set; }
        public int Resource1Amount { get; set; }
        public long? Resource2Id { get; set; }
        public int? Resource2Amount { get; set; }
        public long? Resource3Id { get; set; }
        public int? Resource3Amount { get; set; }
        public int GainMedal { get; set; } // All 1
    }
}
