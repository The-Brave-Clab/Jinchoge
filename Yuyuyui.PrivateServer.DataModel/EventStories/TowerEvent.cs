using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class TowerEvent
    {
        public byte[]? Id { get; set; }
        public byte[]? SpecialChapterId { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? Act { get; set; }
        public byte[]? AvailableUserLevel { get; set; }
        public byte[]? AvailableUserCardLevel { get; set; }
        public byte[]? UsableCountOfAUserCardPerDay { get; set; }
        public byte[]? SecondsForRecoverUserCardsFromStartOfDay { get; set; }
        public byte[]? StartAt { get; set; }
        public byte[]? EndAt { get; set; }
        public byte[]? TestStartEndAt { get; set; }
        public byte[]? TestEndAt { get; set; }
    }
}
