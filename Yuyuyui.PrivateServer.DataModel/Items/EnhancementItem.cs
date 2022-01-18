using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class EnhancementItem
    {
        public byte[]? Id { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? Description { get; set; }
        public byte[]? Exp { get; set; }
        public byte[]? CostCoefficient { get; set; }
        public byte[]? DisposalPrice { get; set; }
        public byte[]? ActiveSkillLevelPotential { get; set; }
        public byte[]? SupportSkillLevelPotential { get; set; }
        public byte[]? SupportSkillLevelCategory { get; set; }
        public byte[]? ImageId { get; set; }
        public byte[]? Rarity { get; set; }
        public byte[]? AssistLevelPotential { get; set; }
        public byte[]? Priority { get; set; }
        public byte[]? AvailableCharacterId1 { get; set; }
        public byte[]? AvailableCharacterId2 { get; set; }
    }
}
