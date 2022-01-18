using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class BraveSystemComponentRecipe
    {
        public long Id { get; set; }
        public int Cost { get; set; }
        public long? Resource1Id { get; set; } // All null?
        public int? Resource1Amount { get; set; } // All null?
        public long? Resource2Id { get; set; } // All null?
        public int? Resource2Amount { get; set; } // All null?
        public long? Resource3Id { get; set; } // All null?
        public int? Resource3Amount { get; set; } // All null?
    }
}
