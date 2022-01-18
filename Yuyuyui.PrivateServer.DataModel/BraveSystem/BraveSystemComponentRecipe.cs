using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class BraveSystemComponentRecipe
    {
        public byte[]? Id { get; set; }
        public byte[]? Cost { get; set; }
        public byte[]? Resource1Id { get; set; }
        public byte[]? Resource1Amount { get; set; }
        public byte[]? Resource2Id { get; set; }
        public byte[]? Resource2Amount { get; set; }
        public byte[]? Resource3Id { get; set; }
        public byte[]? Resource3Amount { get; set; }
    }
}
