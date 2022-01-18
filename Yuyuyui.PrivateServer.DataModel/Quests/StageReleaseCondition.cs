using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class StageReleaseCondition // Consider merge with SpecialStageCondition
    {
        public long Id { get; set; }
        public long StageId { get; set; }
        public long? FinishStageId { get; set; }
    }
}
