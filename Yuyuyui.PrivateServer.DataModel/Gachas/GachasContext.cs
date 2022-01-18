using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class GachasContext : BaseContext<GachasContext>
    {
        public GachasContext()
        {
        }

        public GachasContext(DbContextOptions<GachasContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Gacha> Gachas { get; set; } = null!;
        public virtual DbSet<GachaBox> GachaBoxes { get; set; } = null!;
        public virtual DbSet<GachaContent> GachaContents { get; set; } = null!;
        public virtual DbSet<GachaLineup> GachaLineups { get; set; } = null!;
        public virtual DbSet<GachaTicket> GachaTickets { get; set; } = null!;

        protected override string DatabaseFileName => "gachas";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gacha>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("gachas");

                entity.Property(e => e.CountDownGacha).HasColumnName("count_down_gacha");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.EndAt).HasColumnName("end_at");

                entity.Property(e => e.GachaTopicId).HasColumnName("gacha_topic_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Kind).HasColumnName("kind");

                entity.Property(e => e.MaxUserLevel).HasColumnName("max_user_level");

                entity.Property(e => e.MinUserLevel).HasColumnName("min_user_level");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.NoDisplayEndAt).HasColumnName("no_display_end_at");

                entity.Property(e => e.NoTicketDisplay).HasColumnName("no_ticket_display");

                entity.Property(e => e.PickupId).HasColumnName("pickup_id");

                entity.Property(e => e.PickupType).HasColumnName("pickup_type");

                entity.Property(e => e.PopupSeName).HasColumnName("popup_se_name");

                entity.Property(e => e.SelectCount).HasColumnName("select_count");

                entity.Property(e => e.SelectGacha).HasColumnName("select_gacha");

                entity.Property(e => e.SkipType).HasColumnName("skip_type");

                entity.Property(e => e.SpecialGet).HasColumnName("special_get");

                entity.Property(e => e.SpecialGetCount).HasColumnName("special_get_count");

                entity.Property(e => e.SpecialSaveRarity).HasColumnName("special_save_rarity");

                entity.Property(e => e.SpecialSelect).HasColumnName("special_select");

                entity.Property(e => e.StartAt).HasColumnName("start_at");

                entity.Property(e => e.StepupGroup).HasColumnName("stepup_group");

                entity.Property(e => e.StepupLoop).HasColumnName("stepup_loop");

                entity.Property(e => e.StepupOrder).HasColumnName("stepup_order");

                entity.Property(e => e.TestEndAt).HasColumnName("test_end_at");

                entity.Property(e => e.TestStartAt).HasColumnName("test_start_at");
            });

            modelBuilder.Entity<GachaBox>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("gacha_boxes");

                entity.Property(e => e.Category1Weight).HasColumnName("category_1_weight");

                entity.Property(e => e.Category2Weight).HasColumnName("category_2_weight");

                entity.Property(e => e.Category3Weight).HasColumnName("category_3_weight");

                entity.Property(e => e.Category4Weight).HasColumnName("category_4_weight");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<GachaContent>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("gacha_contents");

                entity.Property(e => e.Category).HasColumnName("category");

                entity.Property(e => e.ContentId).HasColumnName("content_id");

                entity.Property(e => e.ContentType).HasColumnName("content_type");

                entity.Property(e => e.GachaBoxId).HasColumnName("gacha_box_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Remarks).HasColumnName("remarks");

                entity.Property(e => e.SelectId).HasColumnName("select_id");

                entity.Property(e => e.SpecialWeightGroup).HasColumnName("special_weight_group");

                entity.Property(e => e.Weight).HasColumnName("weight");
            });

            modelBuilder.Entity<GachaLineup>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("gacha_lineups");

                entity.Property(e => e.ButtonExtra).HasColumnName("button_extra");

                entity.Property(e => e.ButtonTitle).HasColumnName("button_title");

                entity.Property(e => e.ConsumptionAmount).HasColumnName("consumption_amount");

                entity.Property(e => e.ConsumptionResourceId).HasColumnName("consumption_resource_id");

                entity.Property(e => e.CountDownGacha).HasColumnName("count_down_gacha");

                entity.Property(e => e.CountUpGacha).HasColumnName("count_up_gacha");

                entity.Property(e => e.DailyLimit).HasColumnName("daily_limit");

                entity.Property(e => e.FreeRareGacha).HasColumnName("free_rare_gacha");

                entity.Property(e => e.GachaBoxId).HasColumnName("gacha_box_id");

                entity.Property(e => e.GachaId).HasColumnName("gacha_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.LotCount).HasColumnName("lot_count");

                entity.Property(e => e.Onetime).HasColumnName("onetime");

                entity.Property(e => e.Pc).HasColumnName("pc");

                entity.Property(e => e.Sp).HasColumnName("sp");
            });

            modelBuilder.Entity<GachaTicket>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("gacha_tickets");

                entity.Property(e => e.ConsumptionResourceId).HasColumnName("consumption_resource_id");

                entity.Property(e => e.GachaId).HasColumnName("gacha_id");

                entity.Property(e => e.GachaKind).HasColumnName("gacha_kind");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.Name).HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
