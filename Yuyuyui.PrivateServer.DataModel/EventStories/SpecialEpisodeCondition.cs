using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SpecialEpisodeCondition
    {
        public byte[]? Id { get; set; }
        public byte[]? SpecialEpisodeId { get; set; }
        public byte[]? FinishSpecialEpisodeId { get; set; }
        public byte[]? StartAt { get; set; }
        public byte[]? EndAt { get; set; }
    }
}
