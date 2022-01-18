using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ActivityRequestSheet
    {
        public byte[]? Id { get; set; }
        public byte[]? Title { get; set; }
        public byte[]? Time { get; set; }
        public byte[]? Description { get; set; }
        public byte[]? ImageId { get; set; }
    }
}
