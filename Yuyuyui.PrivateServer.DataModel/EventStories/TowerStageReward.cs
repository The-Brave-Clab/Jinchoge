using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class TowerStageReward
    {
        public long Id { get; set; }
        public long TowerStageId { get; set; }
        public long ContentId { get; set; }
        public string ContentType { get; set; }
        public int Quantity { get; set; }
    }
}
