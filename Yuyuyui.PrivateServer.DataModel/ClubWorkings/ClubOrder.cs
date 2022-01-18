using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ClubOrder
    {
        public byte[]? Id { get; set; }
        public byte[]? Title { get; set; }
        public byte[]? Description { get; set; }
        public byte[]? Rarity { get; set; }
        public byte[]? Duration { get; set; }
        public byte[]? RewardBox1Id { get; set; }
        public byte[]? RewardBox2Id { get; set; }
        public byte[]? RewardBox3Id { get; set; }
        public byte[]? FamiliarityExp { get; set; }
        public byte[]? ExpiredAt { get; set; }
    }
}
