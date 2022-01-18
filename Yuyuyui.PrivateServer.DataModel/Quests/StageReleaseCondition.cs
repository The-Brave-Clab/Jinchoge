using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class StageReleaseCondition
    {
        public byte[]? Id { get; set; }
        public byte[]? StageId { get; set; }
        public byte[]? FinishStageId { get; set; }
    }
}
