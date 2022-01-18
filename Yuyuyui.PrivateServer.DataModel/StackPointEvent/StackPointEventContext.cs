using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class StackPointEventContext : BaseContext<StackPointEventContext>
    {
        public StackPointEventContext()
        {
        }

        public StackPointEventContext(DbContextOptions<StackPointEventContext> options)
            : base(options)
        {
        }

        public virtual DbSet<StackPointEventReward> StackPointEventRewards { get; set; } = null!;

        protected override string DatabaseFileName => "stack_point_event";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StackPointEventReward>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("stack_point_event_rewards");

                entity.Property(e => e.ContentId).HasColumnName("content_id");

                entity.Property(e => e.ContentType).HasColumnName("content_type");

                entity.Property(e => e.GiftId).HasColumnName("gift_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.NeedPoint).HasColumnName("need_point");

                entity.Property(e => e.PickUp).HasColumnName("pick_up");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.StackPointEventId).HasColumnName("stack_point_event_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
