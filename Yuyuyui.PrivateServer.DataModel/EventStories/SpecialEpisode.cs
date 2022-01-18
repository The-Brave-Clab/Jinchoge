using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SpecialEpisode
    {
        public byte[]? Id { get; set; }
        public byte[]? SpecialChapterId { get; set; }
        public byte[]? Number { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? MapMoveCharacterId { get; set; }
        public byte[]? BackgroundId { get; set; }
        public byte[]? StartQuestId { get; set; }
        public byte[]? StartSpecialStageId { get; set; }
        public byte[]? EpisodeNaviId { get; set; }
        public byte[]? NaviMessage { get; set; }
        public byte[]? Element { get; set; }
    }
}
