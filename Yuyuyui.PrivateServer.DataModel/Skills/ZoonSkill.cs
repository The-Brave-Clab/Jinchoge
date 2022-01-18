using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ZoonSkill
    {
        public byte[]? Id { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? Detail { get; set; }
        public byte[]? PrefabName { get; set; }
        public byte[]? AreaType { get; set; }
        public byte[]? Part { get; set; }
        public byte[]? PartMin { get; set; }
        public byte[]? PartMax { get; set; }
    }
}
