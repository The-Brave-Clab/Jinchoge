using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SpecialAttackCharacter
    {
        public byte[]? Id { get; set; }
        public byte[]? SpecialChapterId { get; set; }
        public byte[]? CharacterId { get; set; }
        public byte[]? ContentType { get; set; }
        public byte[]? ContentId { get; set; }
        public byte[]? EffectRate { get; set; }
        public byte[]? Rarity { get; set; }
        public byte[]? StartAt { get; set; }
        public byte[]? EndAt { get; set; }
        public byte[]? TestStartAt { get; set; }
        public byte[]? TestEndAt { get; set; }
    }
}
