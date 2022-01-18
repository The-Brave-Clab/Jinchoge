using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class BingoSquare
    {
        public byte[]? Id { get; set; }
        public byte[]? BingoSheetId { get; set; }
        public byte[]? Position { get; set; }
        public byte[]? BingoOpenItemId { get; set; }
        public byte[]? BingoOpenItemNum { get; set; }
        public byte[]? BingoRewardId { get; set; }
        public byte[]? CartoonFrameId { get; set; }
    }
}
