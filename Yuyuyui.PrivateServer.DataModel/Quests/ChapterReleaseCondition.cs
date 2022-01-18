using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ChapterReleaseCondition
    {
        public byte[]? Id { get; set; }
        public byte[]? ChapterId { get; set; }
        public byte[]? FinishEpisodeId { get; set; }
        public byte[]? ChapterHide { get; set; }
    }
}
