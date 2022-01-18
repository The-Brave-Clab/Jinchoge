using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SkillPart
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int PartType { get; set; }
        public int? Attribute { get; set; }
        public int? AddSpAttribute { get; set; }
        public float? AddSpValue { get; set; }
        public int? AttackType { get; set; }
        public int? CharacterType { get; set; }
        public int? RarityType { get; set; }
        public int? AreaType { get; set; }
        public int? TargetSide { get; set; }
        public int? BuffType { get; set; }
        public int? BuffStatus { get; set; }
        public int? ValueType { get; set; }
        public float? Value { get; set; }
        public int? HitMin { get; set; }
        public int? HitMax { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }
        public float? Angle { get; set; }
        public float? MoveX { get; set; }
        public string? BgEffect { get; set; } // All null, data type unknown
        public long? EffectTime { get; set; }
        public long? ZoonId { get; set; }
        public long? AreaId { get; set; }
    }
}
