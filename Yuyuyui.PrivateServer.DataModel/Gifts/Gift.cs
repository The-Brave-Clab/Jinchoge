using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Gift
    {
        public long Id { get; set; }
        public long ContentId { get; set; }
        public string ContentType { get; set; } = "";
        public long Quantity { get; set; }
        public string Title { get; set; } = "";
    }
}
