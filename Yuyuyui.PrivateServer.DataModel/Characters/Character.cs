using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Character
    {
        public byte[]? Id { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? CvName { get; set; }
        public byte[]? SelfVoiceId { get; set; }
        public byte[]? AWordVoiceId { get; set; }
        public byte[]? MorningVoiceId { get; set; }
        public byte[]? DaytimeVoiceId { get; set; }
        public byte[]? NightVoiceId { get; set; }
        public byte[]? SleepVoiceId { get; set; }
    }
}
