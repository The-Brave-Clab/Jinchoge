using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ActiveSkillLevel
    {
        public byte[]? Id { get; set; }
        public byte[]? LevelCategoy { get; set; }
        public byte[]? Level { get; set; }
        public byte[]? LevelUpParam { get; set; }
    }
}
