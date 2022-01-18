using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SpecialEpisode
    {
        public long Id { get; set; }
        public long SpecialChapterId { get; set; }
        public int Number { get; set; }
        public string? Name { get; set; } = null;
        public long MapMoveCharacterId { get; set; }
        public long BackgroundId { get; set; }
        public long StartQuestId { get; set; }
        public long StartSpecialStageId { get; set; }
        public long? EpisodeNaviId { get; set; } = null;
        public string? NaviMessage { get; set; } = null; // All null
        public int? Element { get; set; } = null;
    }
}
