using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SpecialAttackCharacter
    {
        public long Id { get; set; }
        public long SpecialChapterId { get; set; }
        public long CharacterId { get; set; }
        public string ContentType { get; set; }
        public long ContentId { get; set; }
        public int EffectRate { get; set; }
        public int Rarity { get; set; }
        public string StartAt { get; set; }
        public string EndAt { get; set; }
        public string TestStartAt { get; set; }
        public string TestEndAt { get; set; }
    }
}
