using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ActiveSkillLevel
    {
        public long Id { get; set; }
        public int LevelCategoy { get; set; }
        public int Level { get; set; }
        public float? LevelUpParam { get; set; } = null;
    }
}
