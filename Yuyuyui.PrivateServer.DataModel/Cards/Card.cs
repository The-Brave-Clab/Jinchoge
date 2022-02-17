using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Card
    {
        public long Id { get; set; }
        public long BaseCardId { get; set; }
        public long CharacterId { get; set; }
        public string Name { get; set; } = "";
        public string? Nickname { get; set; } = null;
        public long ImageId { get; set; }
        public int ExtraImagePotential { get; set; }
        public long? PotentialGiftId { get; set; } = null;
        public string? PotentialGiftText { get; set; } = null;
        public int? PotentialGiftBorder { get; set; } = null;
        public long? LeaderSkillId { get; set; } = null;
        public long? ActiveSkillId { get; set; } = null;
        public long? SupportSkillId { get; set; } = null;
        public int AttackType { get; set; }
        public int LevelCategory { get; set; }
        public int GrowthKind { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public int Element { get; set; }
        public int Rarity { get; set; }
        public int Cost { get; set; }
        public int SupportPoint { get; set; }
        public int MinHitPoint { get; set; }
        public int MaxHitPoint { get; set; }
        public int MinAttack { get; set; }
        public int MaxAttack { get; set; }
        public int LevelMaxHitPointBonus { get; set; }
        public int LevelMaxAttackBonus { get; set; }
        public int MinWeight { get; set; }
        public int MaxWeight { get; set; }
        public int MinCritical { get; set; }
        public int MaxCritical { get; set; }
        public int MinAgility { get; set; }
        public int MaxAgility { get; set; }
        public float PotentialHitPointArgument { get; set; }
        public float PotentialAttackArgument { get; set; }
        public float AttackPace { get; set; }
        public long? LevelMaxGiftId { get; set; } = null;
        public int? EvolutionRewardBraveCoin { get; set; } = null;
        public long? EvolutionRecipeId { get; set; } = null;
        public long? EvolutionCardId { get; set; } = null;
        public int EvolutionLevel { get; set; }
        public long? EvolutionRewardAccessory1Id { get; set; } = null;
        public long? EvolutionRewardAccessory2Id { get; set; } = null;
        public long? EvolutionRewardAccessory3Id { get; set; } = null;
        public long? EvolutionRewardAccessory4Id { get; set; } = null;
        public long? EvolutionRewardAccessory5Id { get; set; } = null;
        public string? GetVoice { get; set; } = null;
        public string? HomeVoice01 { get; set; } = null;
        public string? HomeVoice02 { get; set; } = null;
        public string? HomeVoice03 { get; set; } = null;
        public string? ActiveSkillVoiceId { get; set; } = null;
        public string? ActiveSkillVoiceId1 { get; set; } = null;
        public long? LimitBreakRewardAccessoryId { get; set; } = null;
        public long? LimitBreakRewardGiftId { get; set; } = null;
        public string? LimitBreakRewardText { get; set; } = null;
        public int? Scaling { get; set; } = null; // Work like a boolean? 1 for true, null for false
        
        public long? GetEvolutionRewardAccessoryId()
        {
            int index = EvolutionLevel - 2;
            switch (index)
            {
                case 0:
                    return EvolutionRewardAccessory1Id;
                case 1:
                    return EvolutionRewardAccessory2Id;
                case 2:
                    return EvolutionRewardAccessory3Id;
                case 3:
                    return EvolutionRewardAccessory4Id;
                case 4:
                    return LimitBreakRewardAccessoryId;
                default:
                    return -1L;
            }
        }
    }
}
