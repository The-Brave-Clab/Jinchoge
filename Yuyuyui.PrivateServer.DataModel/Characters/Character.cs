using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Character
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? CvName { get; set; }
        public string? SelfVoiceId { get; set; }
        public string? AWordVoiceId { get; set; }
        public string? MorningVoiceId { get; set; }
        public string? DaytimeVoiceId { get; set; }
        public string? NightVoiceId { get; set; }
        public string? SleepVoiceId { get; set; }
    }
}
