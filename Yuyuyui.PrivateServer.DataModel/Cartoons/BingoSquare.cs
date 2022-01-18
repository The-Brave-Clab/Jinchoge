using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class BingoSquare
    {
        public long Id { get; set; }
        public long BingoSheetId { get; set; }
        public int Position { get; set; }
        public long BingoOpenItemId { get; set; }
        public int BingoOpenItemNum { get; set; }
        public long? BingoRewardId { get; set; } // All null
        public long CartoonFrameId { get; set; }
    }
}
