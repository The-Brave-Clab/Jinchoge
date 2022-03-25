using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class StaminaItem
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public int Stamina { get; set; }
        public int AddType { get; set; }
        public string Description { get; set; } = "";
        public long ImageId { get; set; }
        public int MultiUse { get; set; } // 01 boolean
        public int Priority { get; set; }
    }
}
