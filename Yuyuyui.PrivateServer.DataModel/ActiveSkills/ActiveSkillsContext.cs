using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ActiveSkillsContext : BaseContext<ActiveSkillsContext>
    {
        public ActiveSkillsContext()
        {
        }

        public ActiveSkillsContext(DbContextOptions<ActiveSkillsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActiveSkill> ActiveSkills { get; set; } = null!;
        public virtual DbSet<ActiveSkillPart> ActiveSkillParts { get; set; } = null!;

        protected override string DatabaseFileName => "active_skills";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActiveSkill>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("active_skill");

                entity.Property(e => e.Cost).HasColumnName("cost");

                entity.Property(e => e.Detail).HasColumnName("detail");

                entity.Property(e => e.IconName).HasColumnName("icon_name");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Part1).HasColumnName("part_1");

                entity.Property(e => e.Part2).HasColumnName("part_2");

                entity.Property(e => e.Part3).HasColumnName("part_3");

                entity.Property(e => e.Part4).HasColumnName("part_4");

                entity.Property(e => e.PrefabName).HasColumnName("prefab_name");
            });

            modelBuilder.Entity<ActiveSkillPart>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("active_skill_part");

                entity.Property(e => e.Angle).HasColumnName("angle");

                entity.Property(e => e.AreaType).HasColumnName("area_type");

                entity.Property(e => e.Attribute).HasColumnName("attribute");

                entity.Property(e => e.BgEffect).HasColumnName("bg_effect");

                entity.Property(e => e.BuffStatus).HasColumnName("buff_status");

                entity.Property(e => e.BuffType).HasColumnName("buff_type");

                entity.Property(e => e.Height).HasColumnName("height");

                entity.Property(e => e.HitMax).HasColumnName("hit_max");

                entity.Property(e => e.HitMin).HasColumnName("hit_min");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.PartType).HasColumnName("part_type");

                entity.Property(e => e.TargetSide).HasColumnName("target_side");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.Property(e => e.ValueType).HasColumnName("value_type");

                entity.Property(e => e.Width).HasColumnName("width");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
