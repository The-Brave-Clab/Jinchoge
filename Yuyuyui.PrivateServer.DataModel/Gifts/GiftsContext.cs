using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class GiftsContext : BaseContext<GiftsContext>
    {
        public GiftsContext()
        {
        }

        public GiftsContext(DbContextOptions<GiftsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Gift> Gifts { get; set; } = null!;

        protected override string DatabaseFileName => "gifts";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gift>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("gifts");

                entity.Property(e => e.ContentId).HasColumnName("content_id");

                entity.Property(e => e.ContentType).HasColumnName("content_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.Title).HasColumnName("title");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
