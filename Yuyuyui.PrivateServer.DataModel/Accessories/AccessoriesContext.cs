using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class AccessoriesContext : BaseContext<AccessoriesContext>
    {
        public AccessoriesContext()
        {
        }

        public AccessoriesContext(DbContextOptions<AccessoriesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Accessory> Accessories { get; set; } = null!;
        public virtual DbSet<AccessoryLevel> AccessoryLevels { get; set; } = null!;

        protected override string DatabaseFileName => "accessories";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accessory>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("accessories");

                entity.Property(e => e.Element).HasColumnName("element");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MaxAttack).HasColumnName("max_attack");

                entity.Property(e => e.MaxCost).HasColumnName("max_cost");

                entity.Property(e => e.MaxHitPoint).HasColumnName("max_hit_point");

                entity.Property(e => e.MaxLevel).HasColumnName("max_level");

                entity.Property(e => e.MinAttack).HasColumnName("min_attack");

                entity.Property(e => e.MinCost).HasColumnName("min_cost");

                entity.Property(e => e.MinHitPoint).HasColumnName("min_hit_point");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Rarity).HasColumnName("rarity");

                entity.Property(e => e.SkillId).HasColumnName("skill_id");

                entity.Property(e => e.UniqueId).HasColumnName("unique_id");
            });

            modelBuilder.Entity<AccessoryLevel>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("accessory_levels");

                entity.Property(e => e.BraveCoin).HasColumnName("brave_coin");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Level).HasColumnName("level");

                entity.Property(e => e.Money).HasColumnName("money");

                entity.Property(e => e.NeedAmount).HasColumnName("need_amount");

                entity.Property(e => e.Rarity).HasColumnName("rarity");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
