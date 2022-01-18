using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Enemy
    {
        public long Id { get; set; }
        public int EnemyType { get; set; }
        public string Name { get; set; }
        public float ImageScale { get; set; }
        public int Radius { get; set; } // Only 1 and 3
        public int Attack { get; set; }
        public long Hp { get; set; }
        public float AttackRadius { get; set; }
        public float AttackPace { get; set; }
        public float MoveSpeed { get; set; }
        public int Defense { get; set; }
        public float AvoidRate { get; set; }
        public float HitRate { get; set; }
        public int CriticalPoint { get; set; }
        public long FootingPoint { get; set; }
        public int Exp { get; set; }
        public int ExpPerAttack { get; set; }
        public int SizeType { get; set; } // All 0
        public int MoveType { get; set; }
        public int MoveToX { get; set; }
        public int WaitTime { get; set; }
        public int MoveTime { get; set; } // All 0
        public int LcMoveType { get; set; }
        public int LcTriggerType { get; set; }
        public int LcMaxCount { get; set; }
        public int LcInterval { get; set; }
        public long BattleItemId { get; set; }
        public float BattleItemDropRate { get; set; }
        public int Notice { get; set; } // Could be 01 boolean
        public int Vertex { get; set; } // 01 boolean
        public int AttackEffectSize { get; set; }
        public int HpGaugeType { get; set; }
        public float HitEffectHeight { get; set; }
        public long As1Id { get; set; }
        public int As1Level { get; set; }
        public int As1Interval { get; set; }
        public long As1Weight { get; set; }
        public long As2Id { get; set; }
        public int As2Level { get; set; }
        public int As2Interval { get; set; }
        public long As2Weight { get; set; }
        public long As3Id { get; set; }
        public int As3Level { get; set; }
        public int As3Interval { get; set; }
        public long As3Weight { get; set; }
        public long As4Id { get; set; }
        public int As4Level { get; set; }
        public int As4Interval { get; set; }
        public long As4Weight { get; set; }
        public long As5Id { get; set; }
        public int As5Level { get; set; }
        public int As5Interval { get; set; }
        public long As5Weight { get; set; }
        public long Ps1Id { get; set; }
        public int Ps1Level { get; set; }
        public long Ps2Id { get; set; }
        public int Ps2Level { get; set; }
        public long Ps3Id { get; set; }
        public int Ps3Level { get; set; }
        public long Ps4Id { get; set; }
        public int Ps4Level { get; set; }
        public long Ps5Id { get; set; }
        public int Ps5Level { get; set; }
        public int AsFirstInterval { get; set; }
        public int BeforeWait { get; set; }
        public long? SlaveEnemy1Id { get; set; }
        public long? SlaveEnemy2Id { get; set; }
        public int? SummonSizeType { get; set; } // All null
        public int? CharacterType { get; set; }
        public long? MasterId { get; set; }
    }
}