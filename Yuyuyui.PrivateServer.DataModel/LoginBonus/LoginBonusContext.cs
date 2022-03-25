using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class LoginBonusContext : BaseContext<LoginBonusContext>
    {
        public LoginBonusContext()
        {
        }

        public LoginBonusContext(DbContextOptions<LoginBonusContext> options)
            : base(options)
        {
        }

        public virtual DbSet<LoginBonusSheet> LoginBonusSheets { get; set; } = null!;
        public virtual DbSet<LoginBonusSheetColumn> LoginBonusSheetColumns { get; set; } = null!;

        protected override string DatabaseFileName => "login_bonus";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginBonusSheet>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("login_bonus_sheets");

                entity.Property(e => e.ComebackDate).HasColumnName("comeback_date");

                entity.Property(e => e.EndAt).HasColumnName("end_at");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Kind).HasColumnName("kind");

                entity.Property(e => e.MaxProgress).HasColumnName("max_progress");

                entity.Property(e => e.NextSheetId).HasColumnName("next_sheet_id");

                entity.Property(e => e.StartAt).HasColumnName("start_at");

                entity.Property(e => e.TestEndAt).HasColumnName("test_end_at");

                entity.Property(e => e.TestStartAt).HasColumnName("test_start_at");
            });

            modelBuilder.Entity<LoginBonusSheetColumn>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("login_bonus_sheet_columns");

                entity.Property(e => e.GiftId).HasColumnName("gift_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.LoginBonusSheetId).HasColumnName("login_bonus_sheet_id");

                entity.Property(e => e.Progress).HasColumnName("progress");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
