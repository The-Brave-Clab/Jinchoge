using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ChapterReleaseCondition
    {
        public long Id { get; set; }
        public long ChapterId { get; set; }
        public long? FinishEpisodeId { get; set; } = null; // All null
        public string ChapterHide { get; set; } = ""; // boolean, TRUE or FALSE
    }
}
