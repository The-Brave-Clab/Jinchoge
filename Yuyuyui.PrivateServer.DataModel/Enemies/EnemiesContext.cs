using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class EnemiesContext : BaseContext<EnemiesContext>
    {
        public EnemiesContext()
        {
        }

        public EnemiesContext(DbContextOptions<EnemiesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Enemy> Enemies { get; set; } = null!;

        protected override string DatabaseFileName => "enemies";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enemy>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("enemies");

                entity.Property(e => e.As1Id).HasColumnName("as_1_id");

                entity.Property(e => e.As1Interval).HasColumnName("as_1_interval");

                entity.Property(e => e.As1Level).HasColumnName("as_1_level");

                entity.Property(e => e.As1Weight).HasColumnName("as_1_weight");

                entity.Property(e => e.As2Id).HasColumnName("as_2_id");

                entity.Property(e => e.As2Interval).HasColumnName("as_2_interval");

                entity.Property(e => e.As2Level).HasColumnName("as_2_level");

                entity.Property(e => e.As2Weight).HasColumnName("as_2_weight");

                entity.Property(e => e.As3Id).HasColumnName("as_3_id");

                entity.Property(e => e.As3Interval).HasColumnName("as_3_interval");

                entity.Property(e => e.As3Level).HasColumnName("as_3_level");

                entity.Property(e => e.As3Weight).HasColumnName("as_3_weight");

                entity.Property(e => e.As4Id).HasColumnName("as_4_id");

                entity.Property(e => e.As4Interval).HasColumnName("as_4_interval");

                entity.Property(e => e.As4Level).HasColumnName("as_4_level");

                entity.Property(e => e.As4Weight).HasColumnName("as_4_weight");

                entity.Property(e => e.As5Id).HasColumnName("as_5_id");

                entity.Property(e => e.As5Interval).HasColumnName("as_5_interval");

                entity.Property(e => e.As5Level).HasColumnName("as_5_level");

                entity.Property(e => e.As5Weight).HasColumnName("as_5_weight");

                entity.Property(e => e.AsFirstInterval).HasColumnName("as_first_interval");

                entity.Property(e => e.Attack).HasColumnName("attack");

                entity.Property(e => e.AttackEffectSize).HasColumnName("attack_effect_size");

                entity.Property(e => e.AttackPace).HasColumnName("attack_pace");

                entity.Property(e => e.AttackRadius).HasColumnName("attack_radius");

                entity.Property(e => e.AvoidRate).HasColumnName("avoid_rate");

                entity.Property(e => e.BattleItemDropRate).HasColumnName("battle_item_drop_rate");

                entity.Property(e => e.BattleItemId).HasColumnName("battle_item_id");

                entity.Property(e => e.BeforeWait).HasColumnName("before_wait");

                entity.Property(e => e.CharacterType).HasColumnName("character_type");

                entity.Property(e => e.CriticalPoint).HasColumnName("critical_point");

                entity.Property(e => e.Defense).HasColumnName("defense");

                entity.Property(e => e.EnemyType).HasColumnName("enemy_type");

                entity.Property(e => e.Exp).HasColumnName("exp");

                entity.Property(e => e.ExpPerAttack).HasColumnName("exp_per_attack");

                entity.Property(e => e.FootingPoint).HasColumnName("footing_point");

                entity.Property(e => e.HitEffectHeight).HasColumnName("hit_effect_height");

                entity.Property(e => e.HitRate).HasColumnName("hit_rate");

                entity.Property(e => e.Hp).HasColumnName("hp");

                entity.Property(e => e.HpGaugeType).HasColumnName("hp_gauge_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageScale).HasColumnName("image_scale");

                entity.Property(e => e.LcInterval).HasColumnName("lc_interval");

                entity.Property(e => e.LcMaxCount).HasColumnName("lc_max_count");

                entity.Property(e => e.LcMoveType).HasColumnName("lc_move_type");

                entity.Property(e => e.LcTriggerType).HasColumnName("lc_trigger_type");

                entity.Property(e => e.MasterId).HasColumnName("master_id");

                entity.Property(e => e.MoveSpeed).HasColumnName("move_speed");

                entity.Property(e => e.MoveTime).HasColumnName("move_time");

                entity.Property(e => e.MoveToX).HasColumnName("move_to_x");

                entity.Property(e => e.MoveType).HasColumnName("move_type");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Notice).HasColumnName("notice");

                entity.Property(e => e.Ps1Id).HasColumnName("ps_1_id");

                entity.Property(e => e.Ps1Level).HasColumnName("ps_1_level");

                entity.Property(e => e.Ps2Id).HasColumnName("ps_2_id");

                entity.Property(e => e.Ps2Level).HasColumnName("ps_2_level");

                entity.Property(e => e.Ps3Id).HasColumnName("ps_3_id");

                entity.Property(e => e.Ps3Level).HasColumnName("ps_3_level");

                entity.Property(e => e.Ps4Id).HasColumnName("ps_4_id");

                entity.Property(e => e.Ps4Level).HasColumnName("ps_4_level");

                entity.Property(e => e.Ps5Id).HasColumnName("ps_5_id");

                entity.Property(e => e.Ps5Level).HasColumnName("ps_5_level");

                entity.Property(e => e.Radius).HasColumnName("radius");

                entity.Property(e => e.SizeType).HasColumnName("size_type");

                entity.Property(e => e.SlaveEnemy1Id).HasColumnName("slave_enemy_1_id");

                entity.Property(e => e.SlaveEnemy2Id).HasColumnName("slave_enemy_2_id");

                entity.Property(e => e.SummonSizeType).HasColumnName("summon_size_type");

                entity.Property(e => e.Vertex).HasColumnName("vertex");

                entity.Property(e => e.WaitTime).HasColumnName("wait_time");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
