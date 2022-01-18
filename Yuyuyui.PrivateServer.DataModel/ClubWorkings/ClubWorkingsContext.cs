using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ClubWorkingsContext : BaseContext<ClubWorkingsContext>
    {
        public ClubWorkingsContext()
        {
        }

        public ClubWorkingsContext(DbContextOptions<ClubWorkingsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ClubOrder> ClubOrders { get; set; } = null!;
        public virtual DbSet<ClubOrderRewardBox> ClubOrderRewardBoxes { get; set; } = null!;

        protected override string DatabaseFileName => "club_workings";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClubOrder>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("club_orders");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.ExpiredAt).HasColumnName("expired_at");

                entity.Property(e => e.FamiliarityExp).HasColumnName("familiarity_exp");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Rarity).HasColumnName("rarity");

                entity.Property(e => e.RewardBox1Id).HasColumnName("reward_box_1_id");

                entity.Property(e => e.RewardBox2Id).HasColumnName("reward_box_2_id");

                entity.Property(e => e.RewardBox3Id).HasColumnName("reward_box_3_id");

                entity.Property(e => e.Title).HasColumnName("title");
            });

            modelBuilder.Entity<ClubOrderRewardBox>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("club_order_reward_boxes");

                entity.Property(e => e.HasQuestion).HasColumnName("has_question");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ItemCategory).HasColumnName("item_category");

                entity.Property(e => e.ItemMasterId).HasColumnName("item_master_id");

                entity.Property(e => e.Title).HasColumnName("title");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
