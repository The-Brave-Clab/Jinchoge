using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Card
    {
        public byte[]? Id { get; set; }
        public byte[]? BaseCardId { get; set; }
        public byte[]? CharacterId { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? Nickname { get; set; }
        public byte[]? ImageId { get; set; }
        public byte[]? ExtraImagePotential { get; set; }
        public byte[]? PotentialGiftId { get; set; }
        public byte[]? PotentialGiftText { get; set; }
        public byte[]? PotentialGiftBorder { get; set; }
        public byte[]? LeaderSkillId { get; set; }
        public byte[]? ActiveSkillId { get; set; }
        public byte[]? SupportSkillId { get; set; }
        public byte[]? AttackType { get; set; }
        public byte[]? LevelCategory { get; set; }
        public byte[]? GrowthKind { get; set; }
        public byte[]? MinLevel { get; set; }
        public byte[]? MaxLevel { get; set; }
        public byte[]? Element { get; set; }
        public byte[]? Rarity { get; set; }
        public byte[]? Cost { get; set; }
        public byte[]? SupportPoint { get; set; }
        public byte[]? MinHitPoint { get; set; }
        public byte[]? MaxHitPoint { get; set; }
        public byte[]? MinAttack { get; set; }
        public byte[]? MaxAttack { get; set; }
        public byte[]? LevelMaxHitPointBonus { get; set; }
        public byte[]? LevelMaxAttackBonus { get; set; }
        public byte[]? MinWeight { get; set; }
        public byte[]? MaxWeight { get; set; }
        public byte[]? MinCritical { get; set; }
        public byte[]? MaxCritical { get; set; }
        public byte[]? MinAgility { get; set; }
        public byte[]? MaxAgility { get; set; }
        public byte[]? PotentialHitPointArgument { get; set; }
        public byte[]? PotentialAttackArgument { get; set; }
        public byte[]? AttackPace { get; set; }
        public byte[]? LevelMaxGiftId { get; set; }
        public byte[]? EvolutionRewardBraveCoin { get; set; }
        public byte[]? EvolutionRecipeId { get; set; }
        public byte[]? EvolutionCardId { get; set; }
        public byte[]? EvolutionLevel { get; set; }
        public byte[]? EvolutionRewardAccessory1Id { get; set; }
        public byte[]? EvolutionRewardAccessory2Id { get; set; }
        public byte[]? EvolutionRewardAccessory3Id { get; set; }
        public byte[]? EvolutionRewardAccessory4Id { get; set; }
        public byte[]? EvolutionRewardAccessory5Id { get; set; }
        public byte[]? GetVoice { get; set; }
        public byte[]? HomeVoice01 { get; set; }
        public byte[]? HomeVoice02 { get; set; }
        public byte[]? HomeVoice03 { get; set; }
        public byte[]? ActiveSkillVoiceId { get; set; }
        public byte[]? ActiveSkillVoiceId1 { get; set; }
        public byte[]? LimitBreakRewardAccessoryId { get; set; }
        public byte[]? LimitBreakRewardGiftId { get; set; }
        public byte[]? LimitBreakRewardText { get; set; }
        public byte[]? Scaling { get; set; }
    }
}
