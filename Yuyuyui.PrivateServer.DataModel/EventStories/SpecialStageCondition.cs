using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SpecialStageCondition
    {
        public long Id { get; set; }
        public long SpecialStageId { get; set; }
        public long? FinishSpecialStageId { get; set; } = null;
    }
}
