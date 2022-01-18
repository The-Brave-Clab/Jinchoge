using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Episode
    {
        public byte[]? Id { get; set; }
        public byte[]? ChapterId { get; set; }
        public byte[]? Number { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? BackgroundId { get; set; }
        public byte[]? StartQuestId { get; set; }
        public byte[]? StartStageId { get; set; }
        public byte[]? GoogleplayAchievementId { get; set; }
        public byte[]? EpisodeNaviId { get; set; }
        public byte[]? NaviMessage { get; set; }
        public byte[]? Element { get; set; }
    }
}
