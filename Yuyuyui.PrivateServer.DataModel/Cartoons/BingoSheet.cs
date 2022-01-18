using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class BingoSheet
    {
        public byte[]? Id { get; set; }
        public byte[]? Priority { get; set; }
        public byte[]? CartoonChapterId { get; set; }
        public byte[]? StartAt { get; set; }
        public byte[]? EndAt { get; set; }
        public byte[]? TestStartAt { get; set; }
        public byte[]? TestEndAt { get; set; }
        public byte[]? BingoRewardId { get; set; }
    }
}
