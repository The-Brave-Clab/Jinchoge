using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SupportSkillLevel
    {
        public long Id { get; set; }
        public int SupportSkillLevelCategory { get; set; }
        public int Level { get; set; }
        public int? LevelUpParam { get; set; }
    }
}
