using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class BingoSheet
    {
        public long Id { get; set; }
        public long Priority { get; set; }
        public long CartoonChapterId { get; set; }
        public string StartAt { get; set; }
        public string EndAt { get; set; }
        public string TestStartAt { get; set; }
        public string TestEndAt { get; set; }
        public long BingoRewardId { get; set; }
    }
}
