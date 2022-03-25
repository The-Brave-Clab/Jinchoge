using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Character
    {
        public long Id { get; set; }
        public string? Name { get; set; } = null;
        public string? CvName { get; set; } = null;
        public string? SelfVoiceId { get; set; } = null;
        public string? AWordVoiceId { get; set; } = null;
        public string? MorningVoiceId { get; set; } = null;
        public string? DaytimeVoiceId { get; set; } = null;
        public string? NightVoiceId { get; set; } = null;
        public string? SleepVoiceId { get; set; } = null;
    }
}
