using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class TowerEvent
    {
        public long Id { get; set; }
        public long SpecialChapterId { get; set; }
        public string Name { get; set; } = "";
        public int Act { get; set; }
        public int AvailableUserLevel { get; set; }
        public int AvailableUserCardLevel { get; set; }
        public int UsableCountOfAUserCardPerDay { get; set; }
        public int SecondsForRecoverUserCardsFromStartOfDay { get; set; }
        public string StartAt { get; set; } = "";
        public string EndAt { get; set; } = "";
        public string TestStartEndAt { get; set; } = "";
        public string TestEndAt { get; set; } = "";
    }
}
