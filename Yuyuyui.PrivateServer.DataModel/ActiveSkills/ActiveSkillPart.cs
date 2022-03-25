using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ActiveSkillPart
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public int PartType { get; set; }
        public int Attribute { get; set; }
        public int? AreaType { get; set; } = null;
        public int? TargetSide { get; set; } = null;
        public int? BuffType { get; set; } = null;
        public int? BuffStatus { get; set; } = null;
        public int? ValueType { get; set; } = null;
        public int? Value { get; set; } = null;
        public int? HitMin { get; set; } = null;
        public int? HitMax { get; set; } = null;
        public float? Width { get; set; } = null;
        public float? Height { get; set; } = null;
        public float? Angle { get; set; } = null;
        public int? BgEffect { get; set; } = null;
    }
}
