using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class EvolutionItem
    {
        public byte[]? Id { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? Description { get; set; }
        public byte[]? TypeName { get; set; }
        public byte[]? Category { get; set; }
        public byte[]? Size { get; set; }
        public byte[]? ImageId { get; set; }
        public byte[]? QuantityLimit { get; set; }
    }
}
