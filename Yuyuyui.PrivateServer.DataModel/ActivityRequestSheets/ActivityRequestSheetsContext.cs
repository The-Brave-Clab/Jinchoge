using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ActivityRequestSheetsContext : BaseContext<ActivityRequestSheetsContext>
    {
        public ActivityRequestSheetsContext()
        {
        }

        public ActivityRequestSheetsContext(DbContextOptions<ActivityRequestSheetsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActivityRequestSheet> ActivityRequestSheets { get; set; } = null!;

        protected override string DatabaseFileName => "activity_request_sheets";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityRequestSheet>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("activity_request_sheets");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.Time).HasColumnName("time");

                entity.Property(e => e.Title).HasColumnName("title");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
