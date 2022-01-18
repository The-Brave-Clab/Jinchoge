using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SupportSkillLevel
    {
        public byte[]? Id { get; set; }
        public byte[]? SupportSkillLevelCategory { get; set; }
        public byte[]? Level { get; set; }
        public byte[]? LevelUpParam { get; set; }
    }
}
