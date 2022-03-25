using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class StageBattle // Consider merge with SpecialStageBattle
    {
        public long Id { get; set; }
        public long StageId { get; set; }
        public int ScoreSpeedExpeditionSecond { get; set; }
    }
}
