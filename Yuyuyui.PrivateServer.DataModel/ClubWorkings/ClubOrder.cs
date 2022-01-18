using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ClubOrder
    {
        public long Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int Rarity { get; set; }
        public long Duration { get; set; }
        public long RewardBox1Id { get; set; }
        public long? RewardBox2Id { get; set; } = null;
        public long? RewardBox3Id { get; set; } = null;
        public int FamiliarityExp { get; set; }
        public string? ExpiredAt { get; set; } = null;
    }
}
