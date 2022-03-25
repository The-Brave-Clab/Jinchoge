using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class PackageItem
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public long ImageId { get; set; }
    }
}
