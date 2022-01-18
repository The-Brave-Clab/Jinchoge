using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SpecialStageBattle
    {
        public long Id { get; set; }
        public long SpecialStageId { get; set; }
        public int ScoreSpeedExpeditionSecond { get; set; }
    }
}
