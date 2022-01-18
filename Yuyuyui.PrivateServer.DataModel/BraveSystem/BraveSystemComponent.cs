using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class BraveSystemComponent
    {
        public long Id { get; set; }
        public int Kind { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxLevel { get; set; }
        public int SkillCategory { get; set; }
        public long ImageId { get; set; }
    }
}
