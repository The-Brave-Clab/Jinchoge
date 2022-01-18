using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ActivityRequestSheet
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public long Time { get; set; }
        public string? Description { get; set; } // ?
        public long ImageId { get; set; }
    }
}
