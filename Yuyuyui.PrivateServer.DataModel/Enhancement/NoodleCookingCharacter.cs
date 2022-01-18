using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class NoodleCookingCharacter
    {
        public long Id { get; set; }
        public long TargetCharacterId { get; set; }
        public string TargetMessageVoiceId { get; set; } = "";
        public string TargetMessage { get; set; } = "";
        public string TargetSpecialMessageVoiceId { get; set; } = "";
        public string TargetSpecialMessage { get; set; } = "";
        public long CookingCharacterId { get; set; }
        public string CookingMessageVoiceId { get; set; } = "";
        public string CookingMessage { get; set; } = "";
        public string CookingSpecialMessageVoiceId { get; set; } = "";
        public string CookingSpecialMessage { get; set; } = "";
        public int Weight { get; set; } // boolean? Only 0 and 1
    }
}
