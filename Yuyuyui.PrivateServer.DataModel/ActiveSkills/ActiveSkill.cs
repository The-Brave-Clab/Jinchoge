using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ActiveSkill
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }
        public int Cost { get; set; }
        public string IconName { get; set; }
        public string PrefabName { get; set; }
        public long Part1 { get; set; }
        public long? Part2 { get; set; }
        public long? Part3 { get; set; }
        public long? Part4 { get; set; }
    }
}
