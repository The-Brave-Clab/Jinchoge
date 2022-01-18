using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SkillsContext : BaseContext<SkillsContext>
    {
        public SkillsContext()
        {
        }

        public SkillsContext(DbContextOptions<SkillsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActiveSkillComplete> ActiveSkills { get; set; } = null!;
        public virtual DbSet<ActiveSkillLevel> ActiveSkillLevels { get; set; } = null!;
        public virtual DbSet<AreaSkill> AreaSkills { get; set; } = null!;
        public virtual DbSet<PassiveSkill> PassiveSkills { get; set; } = null!;
        public virtual DbSet<SkillPart> SkillParts { get; set; } = null!;
        public virtual DbSet<SupportSkillLevel> SupportSkillLevels { get; set; } = null!;
        public virtual DbSet<AreaSkill> ZoonSkills { get; set; } = null!;

        protected override string DatabaseFileName => "skills";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActiveSkillComplete>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("active_skill");

                entity.Property(e => e.AttackEffectSize).HasColumnName("attack_effect_size");

                entity.Property(e => e.CoolTime).HasColumnName("cool_time");

                entity.Property(e => e.Cost).HasColumnName("cost");

                entity.Property(e => e.CutinType).HasColumnName("cutin_type");

                entity.Property(e => e.DeleteDuration).HasColumnName("delete_duration");

                entity.Property(e => e.Detail).HasColumnName("detail");

                entity.Property(e => e.EnemyFirstInterval).HasColumnName("enemy_first_interval");

                entity.Property(e => e.EnemyStopCount).HasColumnName("enemy_stop_count");

                entity.Property(e => e.IconName).HasColumnName("icon_name");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.LevelCategory).HasColumnName("level_category");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Part1).HasColumnName("part_1");

                entity.Property(e => e.Part10).HasColumnName("part_10");

                entity.Property(e => e.Part10Max).HasColumnName("part_10_max");

                entity.Property(e => e.Part10Min).HasColumnName("part_10_min");

                entity.Property(e => e.Part1Max).HasColumnName("part_1_max");

                entity.Property(e => e.Part1Min).HasColumnName("part_1_min");

                entity.Property(e => e.Part2).HasColumnName("part_2");

                entity.Property(e => e.Part2Max).HasColumnName("part_2_max");

                entity.Property(e => e.Part2Min).HasColumnName("part_2_min");

                entity.Property(e => e.Part3).HasColumnName("part_3");

                entity.Property(e => e.Part3Max).HasColumnName("part_3_max");

                entity.Property(e => e.Part3Min).HasColumnName("part_3_min");

                entity.Property(e => e.Part4).HasColumnName("part_4");

                entity.Property(e => e.Part4Max).HasColumnName("part_4_max");

                entity.Property(e => e.Part4Min).HasColumnName("part_4_min");

                entity.Property(e => e.Part5).HasColumnName("part_5");

                entity.Property(e => e.Part5Max).HasColumnName("part_5_max");

                entity.Property(e => e.Part5Min).HasColumnName("part_5_min");

                entity.Property(e => e.Part6).HasColumnName("part_6");

                entity.Property(e => e.Part6Max).HasColumnName("part_6_max");

                entity.Property(e => e.Part6Min).HasColumnName("part_6_min");

                entity.Property(e => e.Part7).HasColumnName("part_7");

                entity.Property(e => e.Part7Max).HasColumnName("part_7_max");

                entity.Property(e => e.Part7Min).HasColumnName("part_7_min");

                entity.Property(e => e.Part8).HasColumnName("part_8");

                entity.Property(e => e.Part8Max).HasColumnName("part_8_max");

                entity.Property(e => e.Part8Min).HasColumnName("part_8_min");

                entity.Property(e => e.Part9).HasColumnName("part_9");

                entity.Property(e => e.Part9Max).HasColumnName("part_9_max");

                entity.Property(e => e.Part9Min).HasColumnName("part_9_min");

                entity.Property(e => e.PrefabMoveX).HasColumnName("prefab_move_x");

                entity.Property(e => e.PrefabName).HasColumnName("prefab_name");

                entity.Property(e => e.PrefabScall).HasColumnName("prefab_scall");

                entity.Property(e => e.Se1Delay).HasColumnName("se_1_delay");

                entity.Property(e => e.Se1Name).HasColumnName("se_1_name");

                entity.Property(e => e.Se2Delay).HasColumnName("se_2_delay");

                entity.Property(e => e.Se2Name).HasColumnName("se_2_name");

                entity.Property(e => e.Se3Delay).HasColumnName("se_3_delay");

                entity.Property(e => e.Se3Name).HasColumnName("se_3_name");

                entity.Property(e => e.SkillType).HasColumnName("skill_type");

                entity.Property(e => e.UrCutscenePrefab).HasColumnName("ur_cutscene_prefab");
            });

            modelBuilder.Entity<ActiveSkillLevel>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("active_skill_levels");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Level).HasColumnName("level");

                entity.Property(e => e.LevelCategoy).HasColumnName("level_categoy");

                entity.Property(e => e.LevelUpParam).HasColumnName("level_up_param");
            });

            modelBuilder.Entity<AreaSkill>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("area_skill");

                entity.Property(e => e.AreaType).HasColumnName("area_type");

                entity.Property(e => e.Detail).HasColumnName("detail");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Part).HasColumnName("part");

                entity.Property(e => e.PartMax).HasColumnName("part_max");

                entity.Property(e => e.PartMin).HasColumnName("part_min");

                entity.Property(e => e.PrefabName).HasColumnName("prefab_name");
            });

            modelBuilder.Entity<PassiveSkill>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("passive_skill");

                entity.Property(e => e.CutinType).HasColumnName("cutin_type");

                entity.Property(e => e.Detail).HasColumnName("detail");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Part1).HasColumnName("part_1");

                entity.Property(e => e.Part10).HasColumnName("part_10");

                entity.Property(e => e.Part10Max).HasColumnName("part_10_max");

                entity.Property(e => e.Part10Min).HasColumnName("part_10_min");

                entity.Property(e => e.Part1Max).HasColumnName("part_1_max");

                entity.Property(e => e.Part1Min).HasColumnName("part_1_min");

                entity.Property(e => e.Part2).HasColumnName("part_2");

                entity.Property(e => e.Part2Max).HasColumnName("part_2_max");

                entity.Property(e => e.Part2Min).HasColumnName("part_2_min");

                entity.Property(e => e.Part3).HasColumnName("part_3");

                entity.Property(e => e.Part3Max).HasColumnName("part_3_max");

                entity.Property(e => e.Part3Min).HasColumnName("part_3_min");

                entity.Property(e => e.Part4).HasColumnName("part_4");

                entity.Property(e => e.Part4Max).HasColumnName("part_4_max");

                entity.Property(e => e.Part4Min).HasColumnName("part_4_min");

                entity.Property(e => e.Part5).HasColumnName("part_5");

                entity.Property(e => e.Part5Max).HasColumnName("part_5_max");

                entity.Property(e => e.Part5Min).HasColumnName("part_5_min");

                entity.Property(e => e.Part6).HasColumnName("part_6");

                entity.Property(e => e.Part6Max).HasColumnName("part_6_max");

                entity.Property(e => e.Part6Min).HasColumnName("part_6_min");

                entity.Property(e => e.Part7).HasColumnName("part_7");

                entity.Property(e => e.Part7Max).HasColumnName("part_7_max");

                entity.Property(e => e.Part7Min).HasColumnName("part_7_min");

                entity.Property(e => e.Part8).HasColumnName("part_8");

                entity.Property(e => e.Part8Max).HasColumnName("part_8_max");

                entity.Property(e => e.Part8Min).HasColumnName("part_8_min");

                entity.Property(e => e.Part9).HasColumnName("part_9");

                entity.Property(e => e.Part9Max).HasColumnName("part_9_max");

                entity.Property(e => e.Part9Min).HasColumnName("part_9_min");

                entity.Property(e => e.PrefabName).HasColumnName("prefab_name");

                entity.Property(e => e.TrigerCount).HasColumnName("triger_count");

                entity.Property(e => e.TrigerPar).HasColumnName("triger_par");

                entity.Property(e => e.TrigerType).HasColumnName("triger_type");

                entity.Property(e => e.TrigerValue).HasColumnName("triger_value");
            });

            modelBuilder.Entity<SkillPart>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("skill_part");

                entity.Property(e => e.AddSpAttribute).HasColumnName("add_sp_attribute");

                entity.Property(e => e.AddSpValue).HasColumnName("add_sp_value");

                entity.Property(e => e.Angle).HasColumnName("angle");

                entity.Property(e => e.AreaId).HasColumnName("area_id");

                entity.Property(e => e.AreaType).HasColumnName("area_type");

                entity.Property(e => e.AttackType).HasColumnName("attack_type");

                entity.Property(e => e.Attribute).HasColumnName("attribute");

                entity.Property(e => e.BgEffect).HasColumnName("bg_effect");

                entity.Property(e => e.BuffStatus).HasColumnName("buff_status");

                entity.Property(e => e.BuffType).HasColumnName("buff_type");

                entity.Property(e => e.CharacterType).HasColumnName("character_type");

                entity.Property(e => e.EffectTime).HasColumnName("effect_time");

                entity.Property(e => e.Height).HasColumnName("height");

                entity.Property(e => e.HitMax).HasColumnName("hit_max");

                entity.Property(e => e.HitMin).HasColumnName("hit_min");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MoveX).HasColumnName("move_x");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.PartType).HasColumnName("part_type");

                entity.Property(e => e.RarityType).HasColumnName("rarity_type");

                entity.Property(e => e.TargetSide).HasColumnName("target_side");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.Property(e => e.ValueType).HasColumnName("value_type");

                entity.Property(e => e.Width).HasColumnName("width");

                entity.Property(e => e.ZoonId).HasColumnName("zoon_id");
            });

            modelBuilder.Entity<SupportSkillLevel>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("support_skill_levels");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Level).HasColumnName("level");

                entity.Property(e => e.LevelUpParam).HasColumnName("level_up_param");

                entity.Property(e => e.SupportSkillLevelCategory).HasColumnName("support_skill_level_category");
            });

            modelBuilder.Entity<AreaSkill>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("zoon_skill");

                entity.Property(e => e.AreaType).HasColumnName("area_type");

                entity.Property(e => e.Detail).HasColumnName("detail");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Part).HasColumnName("part");

                entity.Property(e => e.PartMax).HasColumnName("part_max");

                entity.Property(e => e.PartMin).HasColumnName("part_min");

                entity.Property(e => e.PrefabName).HasColumnName("prefab_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
