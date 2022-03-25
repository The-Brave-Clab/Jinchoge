using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class CharactersContext : BaseContext<CharactersContext>
    {
        public CharactersContext()
        {
        }

        public CharactersContext(DbContextOptions<CharactersContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Character> Characters { get; set; } = null!;
        public virtual DbSet<FamiliarityLevel> FamiliarityLevels { get; set; } = null!;

        protected override string DatabaseFileName => "characters";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Character>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("characters");

                entity.Property(e => e.AWordVoiceId).HasColumnName("a_word_voice_id");

                entity.Property(e => e.CvName).HasColumnName("cv_name");

                entity.Property(e => e.DaytimeVoiceId).HasColumnName("daytime_voice_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MorningVoiceId).HasColumnName("morning_voice_id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.NightVoiceId).HasColumnName("night_voice_id");

                entity.Property(e => e.SelfVoiceId).HasColumnName("self_voice_id");

                entity.Property(e => e.SleepVoiceId).HasColumnName("sleep_voice_id");
            });

            modelBuilder.Entity<FamiliarityLevel>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("familiarity_levels");

                entity.Property(e => e.AttackCoefficient).HasColumnName("attack_coefficient");

                entity.Property(e => e.HitPointCoefficient).HasColumnName("hit_point_coefficient");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Level).HasColumnName("level");

                entity.Property(e => e.MaxExp).HasColumnName("max_exp");

                entity.Property(e => e.SupportPointBonus).HasColumnName("support_point_bonus");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
