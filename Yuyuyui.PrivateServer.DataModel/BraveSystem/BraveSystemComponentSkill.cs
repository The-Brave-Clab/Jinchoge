using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class BraveSystemComponentSkill
    {
        public long Id { get; set; }
        public int Category { get; set; }
        public int Level { get; set; }
        public long? BraveSystemComponentRecipeId { get; set; }
        public long? SkillId { get; set; } // All null?
        public int? SkillLevel { get; set; } // All null?
    }
}
