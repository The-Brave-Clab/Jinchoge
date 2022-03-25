using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class TitleItem
    {
        public long Id { get; set; }
        public long? Priority { get; set; } = null; // looks like card master id
        public string Name { get; set; } = "";
        public int ContentType { get; set; }
        public int Rarity { get; set; }
        public string Description { get; set; } = "";
        public string? UpgradeDescription { get; set; } = null; // All null
        public long? OpenCondition { get; set; } = null; // looks like card master id
        public string? OpenValues { get; set; } = null; // only "99,1" (for cards) and null
        public int UpgradeRelation { get; set; } // All 0
        public long? NextId { get; set; } = null; // All null
        public long BoardImageId { get; set; }
        public long TextImageId { get; set; } // This is long enough, but be careful
        public long? CharacterLeft { get; set; } = null; // All null, character id?
        public long? CharacterRight { get; set; } = null; // All null, character id?
        public long? CharacterId { get; set; } = null;
        public string? StartAt { get; set; } = null;
        public string? EndAt { get; set; } = null; // All null
        public string? TestStartAt { get; set; } = null; // All null
        public string? TestEndAt { get; set; } = null; // All null
    }
}