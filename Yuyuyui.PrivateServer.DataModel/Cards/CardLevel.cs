using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class CardLevel
    {
        public byte[]? Id { get; set; }
        public byte[]? LevelCategory { get; set; }
        public byte[]? Level { get; set; }
        public byte[]? MaxExp { get; set; }
    }
}
