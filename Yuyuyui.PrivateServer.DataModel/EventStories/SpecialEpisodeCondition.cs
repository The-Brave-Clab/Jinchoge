using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SpecialEpisodeCondition
    {
        public long Id { get; set; }
        public long SpecialEpisodeId { get; set; }
        public long? FinishSpecialEpisodeId { get; set; }
        public string? StartAt { get; set; }
        public string? EndAt { get; set; }
    }
}
