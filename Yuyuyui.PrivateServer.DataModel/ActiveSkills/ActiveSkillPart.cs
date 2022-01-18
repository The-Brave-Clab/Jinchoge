using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ActiveSkillPart
    {
        public byte[]? Id { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? PartType { get; set; }
        public byte[]? Attribute { get; set; }
        public byte[]? AreaType { get; set; }
        public byte[]? TargetSide { get; set; }
        public byte[]? BuffType { get; set; }
        public byte[]? BuffStatus { get; set; }
        public byte[]? ValueType { get; set; }
        public byte[]? Value { get; set; }
        public byte[]? HitMin { get; set; }
        public byte[]? HitMax { get; set; }
        public byte[]? Width { get; set; }
        public byte[]? Height { get; set; }
        public byte[]? Angle { get; set; }
        public byte[]? BgEffect { get; set; }
    }
}
