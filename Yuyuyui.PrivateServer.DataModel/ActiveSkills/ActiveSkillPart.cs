using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ActiveSkillPart
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int PartType { get; set; }
        public int Attribute { get; set; }
        public int? AreaType { get; set; }
        public int? TargetSide { get; set; }
        public int? BuffType { get; set; }
        public int? BuffStatus { get; set; }
        public int? ValueType { get; set; }
        public int? Value { get; set; }
        public int? HitMin { get; set; }
        public int? HitMax { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }
        public float? Angle { get; set; }
        public int? BgEffect { get; set; }
    }
}
