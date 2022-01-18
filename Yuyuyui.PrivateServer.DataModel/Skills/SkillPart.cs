using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SkillPart
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public int PartType { get; set; }
        public int? Attribute { get; set; } = null;
        public int? AddSpAttribute { get; set; } = null;
        public float? AddSpValue { get; set; } = null;
        public int? AttackType { get; set; } = null;
        public int? CharacterType { get; set; } = null;
        public int? RarityType { get; set; } = null;
        public int? AreaType { get; set; } = null;
        public int? TargetSide { get; set; } = null;
        public int? BuffType { get; set; } = null;
        public int? BuffStatus { get; set; } = null;
        public int? ValueType { get; set; } = null;
        public float? Value { get; set; } = null;
        public int? HitMin { get; set; } = null;
        public int? HitMax { get; set; } = null;
        public float? Width { get; set; } = null;
        public float? Height { get; set; } = null;
        public float? Angle { get; set; } = null;
        public float? MoveX { get; set; } = null;
        public string? BgEffect { get; set; } = null; // All null, data type unknown
        public long? EffectTime { get; set; } = null;
        public long? ZoonId { get; set; } = null;
        public long? AreaId { get; set; } = null;
    }
}
