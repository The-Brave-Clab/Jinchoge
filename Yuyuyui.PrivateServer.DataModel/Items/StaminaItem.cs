using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class StaminaItem
    {
        public byte[]? Id { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? Stamina { get; set; }
        public byte[]? AddType { get; set; }
        public byte[]? Description { get; set; }
        public byte[]? ImageId { get; set; }
        public byte[]? MultiUse { get; set; }
        public byte[]? Priority { get; set; }
    }
}
