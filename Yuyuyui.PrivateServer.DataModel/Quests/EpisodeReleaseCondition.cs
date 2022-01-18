using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class EpisodeReleaseCondition
    {
        public long Id { get; set; }
        public long EpisodeId { get; set; }
        public long? FinishEpisodeId { get; set; }
    }
}
