using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class EnhancementItem
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int Exp { get; set; }
        public int CostCoefficient { get; set; }
        public int DisposalPrice { get; set; }
        public int ActiveSkillLevelPotential { get; set; }
        public int SupportSkillLevelPotential { get; set; }
        public int SupportSkillLevelCategory { get; set; }
        public long ImageId { get; set; }
        public int Rarity { get; set; }
        public int AssistLevelPotential { get; set; }
        public long Priority { get; set; }
        public long? AvailableCharacterId1 { get; set; } = null;
        public long? AvailableCharacterId2 { get; set; } = null;
    }
}
