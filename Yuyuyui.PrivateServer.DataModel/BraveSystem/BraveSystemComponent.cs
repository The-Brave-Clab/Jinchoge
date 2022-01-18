using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class BraveSystemComponent
    {
        public byte[]? Id { get; set; }
        public byte[]? Kind { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? Description { get; set; }
        public byte[]? MaxLevel { get; set; }
        public byte[]? SkillCategory { get; set; }
        public byte[]? ImageId { get; set; }
    }
}
