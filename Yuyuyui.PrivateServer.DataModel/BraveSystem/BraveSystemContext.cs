using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class BraveSystemContext : BaseContext<BraveSystemContext>
    {
        public BraveSystemContext()
        {
        }

        public BraveSystemContext(DbContextOptions<BraveSystemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BraveSystemComponent> BraveSystemComponents { get; set; } = null!;
        public virtual DbSet<BraveSystemComponentEffect> BraveSystemComponentEffects { get; set; } = null!;
        public virtual DbSet<BraveSystemComponentRecipe> BraveSystemComponentRecipes { get; set; } = null!;
        public virtual DbSet<BraveSystemComponentSkill> BraveSystemComponentSkills { get; set; } = null!;

        protected override string DatabaseFileName => "brave_system";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BraveSystemComponent>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("brave_system_components");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.Kind).HasColumnName("kind");

                entity.Property(e => e.MaxLevel).HasColumnName("max_level");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.SkillCategory).HasColumnName("skill_category");
            });

            modelBuilder.Entity<BraveSystemComponentEffect>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("brave_system_component_effects");

                entity.Property(e => e.BraveSystemComponentId).HasColumnName("brave_system_component_id");

                entity.Property(e => e.Effect).HasColumnName("effect");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Level).HasColumnName("level");
            });

            modelBuilder.Entity<BraveSystemComponentRecipe>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("brave_system_component_recipes");

                entity.Property(e => e.Cost).HasColumnName("cost");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Resource1Amount).HasColumnName("resource_1_amount");

                entity.Property(e => e.Resource1Id).HasColumnName("resource_1_id");

                entity.Property(e => e.Resource2Amount).HasColumnName("resource_2_amount");

                entity.Property(e => e.Resource2Id).HasColumnName("resource_2_id");

                entity.Property(e => e.Resource3Amount).HasColumnName("resource_3_amount");

                entity.Property(e => e.Resource3Id).HasColumnName("resource_3_id");
            });

            modelBuilder.Entity<BraveSystemComponentSkill>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("brave_system_component_skills");

                entity.Property(e => e.BraveSystemComponentRecipeId).HasColumnName("brave_system_component_recipe_id");

                entity.Property(e => e.Category).HasColumnName("category");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Level).HasColumnName("level");

                entity.Property(e => e.SkillId).HasColumnName("skill_id");

                entity.Property(e => e.SkillLevel).HasColumnName("skill_level");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
