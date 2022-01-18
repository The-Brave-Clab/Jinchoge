using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class EvolutionItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? TypeName { get; set; }
        public string? Category { get; set; }
        public string Size { get; set; } // 大中小
        public long ImageId { get; set; }
        public long QuantityLimit { get; set; }
    }
}
