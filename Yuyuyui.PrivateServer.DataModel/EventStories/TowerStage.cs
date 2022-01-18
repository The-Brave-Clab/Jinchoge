using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class TowerStage
    {
        public long Id { get; set; }
        public long TowerEventId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public long SpecialStageId { get; set; }
    }
}
