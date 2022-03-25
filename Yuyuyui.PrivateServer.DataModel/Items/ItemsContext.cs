using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class ItemsContext : BaseContext<ItemsContext>
    {
        public ItemsContext()
        {
        }

        public ItemsContext(DbContextOptions<ItemsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActivityRequestSheet> ActivityRequestSheets { get; set; } = null!;
        public virtual DbSet<AutoClearTicket> AutoClearTickets { get; set; } = null!;
        public virtual DbSet<BingoOpenItem> BingoOpenItems { get; set; } = null!;
        public virtual DbSet<EnhancementItem> EnhancementItems { get; set; } = null!;
        public virtual DbSet<EvolutionItem> EvolutionItems { get; set; } = null!;
        public virtual DbSet<Gift> Gifts { get; set; } = null!;
        public virtual DbSet<InstantStaminaItem> InstantStaminaItems { get; set; } = null!;
        public virtual DbSet<PackageItem> PackageItems { get; set; } = null!;
        public virtual DbSet<StaminaItem> StaminaItems { get; set; } = null!;
        public virtual DbSet<TimeLimitedBuff> TimeLimitedBuffs { get; set; } = null!;
        public virtual DbSet<TitleItem> TitleItems { get; set; } = null!;

        protected override string DatabaseFileName => "items";

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

            modelBuilder.Entity<AutoClearTicket>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("auto_clear_tickets");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.SecondsForUsingTimesResetTime).HasColumnName("seconds_for_using_times_reset_time");

                entity.Property(e => e.UsingTimesLimit).HasColumnName("using_times_limit");
            });

            modelBuilder.Entity<BingoOpenItem>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("bingo_open_items");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<EnhancementItem>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("enhancement_items");

                entity.Property(e => e.ActiveSkillLevelPotential).HasColumnName("active_skill_level_potential");

                entity.Property(e => e.AssistLevelPotential).HasColumnName("assist_level_potential");

                entity.Property(e => e.AvailableCharacterId1).HasColumnName("available_character_id_1");

                entity.Property(e => e.AvailableCharacterId2).HasColumnName("available_character_id_2");

                entity.Property(e => e.CostCoefficient).HasColumnName("cost_coefficient");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.DisposalPrice).HasColumnName("disposal_price");

                entity.Property(e => e.Exp).HasColumnName("exp");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.Rarity).HasColumnName("rarity");

                entity.Property(e => e.SupportSkillLevelCategory).HasColumnName("support_skill_level_category");

                entity.Property(e => e.SupportSkillLevelPotential).HasColumnName("support_skill_level_potential");
            });

            modelBuilder.Entity<EvolutionItem>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("evolution_items");

                entity.Property(e => e.Category).HasColumnName("category");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.QuantityLimit).HasColumnName("quantity_limit");

                entity.Property(e => e.Size).HasColumnName("size");

                entity.Property(e => e.TypeName).HasColumnName("type_name");
            });

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

            modelBuilder.Entity<InstantStaminaItem>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("instant_stamina_items");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Stamina).HasColumnName("stamina");
            });

            modelBuilder.Entity<PackageItem>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("package_items");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<StaminaItem>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("stamina_items");

                entity.Property(e => e.AddType).HasColumnName("add_type");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.MultiUse).HasColumnName("multi_use");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.Stamina).HasColumnName("stamina");
            });

            modelBuilder.Entity<TimeLimitedBuff>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("time_limited_buffs");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Kind).HasColumnName("kind");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Rate).HasColumnName("rate");
            });

            modelBuilder.Entity<TitleItem>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("title_items");

                entity.Property(e => e.BoardImageId).HasColumnName("board_image_id");

                entity.Property(e => e.CharacterId).HasColumnName("character_id");

                entity.Property(e => e.CharacterLeft).HasColumnName("character_left");

                entity.Property(e => e.CharacterRight).HasColumnName("character_right");

                entity.Property(e => e.ContentType).HasColumnName("content_type");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.EndAt).HasColumnName("end_at");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.NextId).HasColumnName("next_id");

                entity.Property(e => e.OpenCondition).HasColumnName("open_condition");

                entity.Property(e => e.OpenValues).HasColumnName("open_values");

                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.Rarity).HasColumnName("rarity");

                entity.Property(e => e.StartAt).HasColumnName("start_at");

                entity.Property(e => e.TestEndAt).HasColumnName("test_end_at");

                entity.Property(e => e.TestStartAt).HasColumnName("test_start_at");

                entity.Property(e => e.TextImageId).HasColumnName("text_image_id");

                entity.Property(e => e.UpgradeDescription).HasColumnName("upgrade_description");

                entity.Property(e => e.UpgradeRelation).HasColumnName("upgrade_relation");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
