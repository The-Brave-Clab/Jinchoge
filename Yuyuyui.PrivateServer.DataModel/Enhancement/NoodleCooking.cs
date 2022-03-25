using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class NoodleCooking
    {
        public long Id { get; set; }
        public long EnhancementItemId { get; set; }
        public long CharacterId { get; set; }
        public long NoodleId { get; set; }
        public long SpecialNoodleId { get; set; }
        public float SpecialHitPercent { get; set; }
    }
}
