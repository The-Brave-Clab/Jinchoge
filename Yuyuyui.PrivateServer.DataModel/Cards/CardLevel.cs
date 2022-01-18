using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class CardLevel
    {
        public long Id { get; set; }
        public int LevelCategory { get; set; }
        public int Level { get; set; }
        public long? MaxExp { get; set; } = null;
    }
}
