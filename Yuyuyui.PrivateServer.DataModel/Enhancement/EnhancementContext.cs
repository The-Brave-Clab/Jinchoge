using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class EnhancementContext : BaseContext<EnhancementContext>
    {
        public EnhancementContext()
        {
        }

        public EnhancementContext(DbContextOptions<EnhancementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Noodle> Noodles { get; set; } = null!;
        public virtual DbSet<NoodleCooking> NoodleCookings { get; set; } = null!;
        public virtual DbSet<NoodleCookingCharacter> NoodleCookingCharacters { get; set; } = null!;

        protected override string DatabaseFileName => "enhancement";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Noodle>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("noodles");

                entity.Property(e => e.ExpCoefficient).HasColumnName("exp_coefficient");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<NoodleCooking>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("noodle_cookings");

                entity.Property(e => e.CharacterId).HasColumnName("character_id");

                entity.Property(e => e.EnhancementItemId).HasColumnName("enhancement_item_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.NoodleId).HasColumnName("noodle_id");

                entity.Property(e => e.SpecialHitPercent).HasColumnName("special_hit_percent");

                entity.Property(e => e.SpecialNoodleId).HasColumnName("special_noodle_id");
            });

            modelBuilder.Entity<NoodleCookingCharacter>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("noodle_cooking_characters");

                entity.Property(e => e.CookingCharacterId).HasColumnName("cooking_character_id");

                entity.Property(e => e.CookingMessage).HasColumnName("cooking_message");

                entity.Property(e => e.CookingMessageVoiceId).HasColumnName("cooking_message_voice_id");

                entity.Property(e => e.CookingSpecialMessage).HasColumnName("cooking_special_message");

                entity.Property(e => e.CookingSpecialMessageVoiceId).HasColumnName("cooking_special_message_voice_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TargetCharacterId).HasColumnName("target_character_id");

                entity.Property(e => e.TargetMessage).HasColumnName("target_message");

                entity.Property(e => e.TargetMessageVoiceId).HasColumnName("target_message_voice_id");

                entity.Property(e => e.TargetSpecialMessage).HasColumnName("target_special_message");

                entity.Property(e => e.TargetSpecialMessageVoiceId).HasColumnName("target_special_message_voice_id");

                entity.Property(e => e.Weight).HasColumnName("weight");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
