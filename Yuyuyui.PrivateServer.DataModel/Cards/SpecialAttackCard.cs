﻿using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SpecialAttackCard
    {
        public long Id { get; set; }
        public long SpecialChapterId { get; set; }
        public long BaseCardId { get; set; }
        public string ContentType { get; set; } = "";
        public long ContentId { get; set; }
        public int EffectRate { get; set; }
        public int PotentialEffectRate { get; set; }
        public int PotentialLimit { get; set; }
        public string? StartAt { get; set; } = null;
        public string? EndAt { get; set; } = null;
        public string? TestStartAt { get; set; } = null;
        public string? TestEndAt { get; set; } = null;
    }
}
