using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class EpisodeReleaseCondition
    {
        public byte[]? Id { get; set; }
        public byte[]? EpisodeId { get; set; }
        public byte[]? FinishEpisodeId { get; set; }
    }
}
