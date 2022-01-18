using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class NoodleCookingCharacter
    {
        public byte[]? Id { get; set; }
        public byte[]? TargetCharacterId { get; set; }
        public byte[]? TargetMessageVoiceId { get; set; }
        public byte[]? TargetMessage { get; set; }
        public byte[]? TargetSpecialMessageVoiceId { get; set; }
        public byte[]? TargetSpecialMessage { get; set; }
        public byte[]? CookingCharacterId { get; set; }
        public byte[]? CookingMessageVoiceId { get; set; }
        public byte[]? CookingMessage { get; set; }
        public byte[]? CookingSpecialMessageVoiceId { get; set; }
        public byte[]? CookingSpecialMessage { get; set; }
        public byte[]? Weight { get; set; }
    }
}
