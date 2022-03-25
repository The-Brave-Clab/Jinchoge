using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ActiveSkillComplete
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string? Detail { get; set; } = null;
        public int Cost { get; set; }
        public string IconName { get; set; } = "";// All 未定
        public string PrefabName { get; set; } = "";
        public int? AttackEffectSize { get; set; } = null; // only 大満開！勇者パーーーンチ！！ is 4, others are null
        public string? UrCutscenePrefab { get; set; } = null;
        public float PrefabScall { get; set; } // I would assume this is actually "Scale"? All 1
        public float DeleteDuration { get; set; }
        public string? Se1Name { get; set; } = null;
        public float? Se1Delay { get; set; } = null; // only 0 and null
        public string? Se2Name { get; set; } = null;
        public float? Se2Delay { get; set; } = null;
        public string? Se3Name { get; set; } = null; // All null
        public float? Se3Delay { get; set; } = null; // All null
        public int CutinType { get; set; }
        public float PrefabMoveX { get; set; }
        public int SkillType { get; set; }
        public int? LevelCategory { get; set; } = null;
        public int CoolTime { get; set; }
        public long? Part1 { get; set; } = null;
        public float? Part1Min { get; set; } = null;
        public float? Part1Max { get; set; } = null;
        public long? Part2 { get; set; } = null;
        public float? Part2Min { get; set; } = null;
        public float? Part2Max { get; set; } = null;
        public long? Part3 { get; set; } = null;
        public float? Part3Min { get; set; } = null;
        public float? Part3Max { get; set; } = null;
        public long? Part4 { get; set; } = null;
        public float? Part4Min { get; set; } = null;
        public float? Part4Max { get; set; } = null;
        public long? Part5 { get; set; } = null;
        public float? Part5Min { get; set; } = null;
        public float? Part5Max { get; set; } = null;
        public long? Part6 { get; set; } = null;
        public float? Part6Min { get; set; } = null;
        public float? Part6Max { get; set; } = null;
        public long? Part7 { get; set; } = null;
        public float? Part7Min { get; set; } = null;
        public float? Part7Max { get; set; } = null;
        public long? Part8 { get; set; } = null;
        public float? Part8Min { get; set; } = null;
        public float? Part8Max { get; set; } = null;
        public long? Part9 { get; set; } = null;
        public float? Part9Min { get; set; } = null;
        public float? Part9Max { get; set; } = null;
        public long? Part10 { get; set; } = null;
        public float? Part10Min { get; set; } = null;
        public float? Part10Max { get; set; } = null;
        public float? EnemyFirstInterval { get; set; } = null;
        public int? EnemyStopCount { get; set; } = null;
    }
}