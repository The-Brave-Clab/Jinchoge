using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class UserLevelsContext : BaseContext<UserLevelsContext>
    {
        public UserLevelsContext()
        {
        }

        public UserLevelsContext(DbContextOptions<UserLevelsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<UserLevel> UserLevels { get; set; } = null!;

        protected override string DatabaseFileName => "user_levels";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserLevel>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("user_levels");

                entity.Property(e => e.EnhancementItemCapacity).HasColumnName("enhancement_item_capacity");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Level).HasColumnName("level");

                entity.Property(e => e.MaxExp).HasColumnName("max_exp");

                entity.Property(e => e.MaxFellow).HasColumnName("max_fellow");

                entity.Property(e => e.MaxStamina).HasColumnName("max_stamina");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
