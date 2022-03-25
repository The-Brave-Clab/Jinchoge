using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class AreaSkill
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Detail { get; set; } = "";
        public string? PrefabName { get; set; } = null; // All null
        public int AreaType { get; set; }
        public long Part { get; set; }
        public float? PartMin { get; set; } = null;
        public float? PartMax { get; set; } = null;
    }
}
