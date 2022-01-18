using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class NoodleCooking
    {
        public byte[]? Id { get; set; }
        public byte[]? EnhancementItemId { get; set; }
        public byte[]? CharacterId { get; set; }
        public byte[]? NoodleId { get; set; }
        public byte[]? SpecialNoodleId { get; set; }
        public byte[]? SpecialHitPercent { get; set; }
    }
}
