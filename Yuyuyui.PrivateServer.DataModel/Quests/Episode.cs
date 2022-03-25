using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Episode
    {
        public long Id { get; set; }
        public long ChapterId { get; set; }
        public int Number { get; set; }
        public string Name { get; set; } = "";
        public long BackgroundId { get; set; }
        public long StartQuestId { get; set; }
        public long StartStageId { get; set; }
        public string? GoogleplayAchievementId { get; set; } = null;
        public long EpisodeNaviId { get; set; } // Looks like character id, but what is 102?
        public string? NaviMessage { get; set; } = null; // All null
        public int Element { get; set; }
    }
}
