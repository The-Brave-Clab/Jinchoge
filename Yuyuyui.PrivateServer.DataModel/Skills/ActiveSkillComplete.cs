using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ActiveSkillComplete
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Detail { get; set; }
        public int Cost { get; set; }
        public string IconName { get; set; } // All 未定
        public string PrefabName { get; set; }
        public int? AttackEffectSize { get; set; } // only 大満開！勇者パーーーンチ！！ is 4, others are null
        public string? UrCutscenePrefab { get; set; }
        public float PrefabScall { get; set; } // I would assume this is actually "Scale"? All 1
        public float DeleteDuration { get; set; }
        public string? Se1Name { get; set; }
        public float? Se1Delay { get; set; } // only 0 and null
        public string? Se2Name { get; set; }
        public float? Se2Delay { get; set; }
        public string? Se3Name { get; set; } // All null
        public float? Se3Delay { get; set; } // All null
        public int CutinType { get; set; }
        public float PrefabMoveX { get; set; }
        public int SkillType { get; set; }
        public int? LevelCategory { get; set; }
        public int CoolTime { get; set; }
        public long? Part1 { get; set; }
        public float? Part1Min { get; set; }
        public float? Part1Max { get; set; }
        public long? Part2 { get; set; }
        public float? Part2Min { get; set; }
        public float? Part2Max { get; set; }
        public long? Part3 { get; set; }
        public float? Part3Min { get; set; }
        public float? Part3Max { get; set; }
        public long? Part4 { get; set; }
        public float? Part4Min { get; set; }
        public float? Part4Max { get; set; }
        public long? Part5 { get; set; }
        public float? Part5Min { get; set; }
        public float? Part5Max { get; set; }
        public long? Part6 { get; set; }
        public float? Part6Min { get; set; }
        public float? Part6Max { get; set; }
        public long? Part7 { get; set; }
        public float? Part7Min { get; set; }
        public float? Part7Max { get; set; }
        public long? Part8 { get; set; }
        public float? Part8Min { get; set; }
        public float? Part8Max { get; set; }
        public long? Part9 { get; set; }
        public float? Part9Min { get; set; }
        public float? Part9Max { get; set; }
        public long? Part10 { get; set; }
        public float? Part10Min { get; set; }
        public float? Part10Max { get; set; }
        public float? EnemyFirstInterval { get; set; }
        public int? EnemyStopCount { get; set; }
    }
}