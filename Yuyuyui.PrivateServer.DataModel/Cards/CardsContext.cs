using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class CardsContext : BaseContext<CardsContext>
    {
        public CardsContext()
        {
        }

        public CardsContext(DbContextOptions<CardsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Card> Cards { get; set; } = null!;
        public virtual DbSet<CardLevel> CardLevels { get; set; } = null!;
        public virtual DbSet<EvolutionRecipe> EvolutionRecipes { get; set; } = null!;
        public virtual DbSet<SpecialAttackCard> SpecialAttackCards { get; set; } = null!;

        protected override string DatabaseFileName => "cards";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("cards");

                entity.Property(e => e.ActiveSkillId).HasColumnName("active_skill_id");

                entity.Property(e => e.ActiveSkillVoiceId).HasColumnName("active_skill_voice_id");

                entity.Property(e => e.ActiveSkillVoiceId1).HasColumnName("active_skill_voice_id1");

                entity.Property(e => e.AttackPace).HasColumnName("attack_pace");

                entity.Property(e => e.AttackType).HasColumnName("attack_type");

                entity.Property(e => e.BaseCardId).HasColumnName("base_card_id");

                entity.Property(e => e.CharacterId).HasColumnName("character_id");

                entity.Property(e => e.Cost).HasColumnName("cost");

                entity.Property(e => e.Element).HasColumnName("element");

                entity.Property(e => e.EvolutionCardId).HasColumnName("evolution_card_id");

                entity.Property(e => e.EvolutionLevel).HasColumnName("evolution_level");

                entity.Property(e => e.EvolutionRecipeId).HasColumnName("evolution_recipe_id");

                entity.Property(e => e.EvolutionRewardAccessory1Id).HasColumnName("evolution_reward_accessory_1_id");

                entity.Property(e => e.EvolutionRewardAccessory2Id).HasColumnName("evolution_reward_accessory_2_id");

                entity.Property(e => e.EvolutionRewardAccessory3Id).HasColumnName("evolution_reward_accessory_3_id");

                entity.Property(e => e.EvolutionRewardAccessory4Id).HasColumnName("evolution_reward_accessory_4_id");

                entity.Property(e => e.EvolutionRewardAccessory5Id).HasColumnName("evolution_reward_accessory_5_id");

                entity.Property(e => e.EvolutionRewardBraveCoin).HasColumnName("evolution_reward_brave_coin");

                entity.Property(e => e.ExtraImagePotential).HasColumnName("extra_image_potential");

                entity.Property(e => e.GetVoice).HasColumnName("get_voice");

                entity.Property(e => e.GrowthKind).HasColumnName("growth_kind");

                entity.Property(e => e.HomeVoice01).HasColumnName("home_voice_01");

                entity.Property(e => e.HomeVoice02).HasColumnName("home_voice_02");

                entity.Property(e => e.HomeVoice03).HasColumnName("home_voice_03");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.LeaderSkillId).HasColumnName("leader_skill_id");

                entity.Property(e => e.LevelCategory).HasColumnName("level_category");

                entity.Property(e => e.LevelMaxAttackBonus).HasColumnName("level_max_attack_bonus");

                entity.Property(e => e.LevelMaxGiftId).HasColumnName("level_max_gift_id");

                entity.Property(e => e.LevelMaxHitPointBonus).HasColumnName("level_max_hit_point_bonus");

                entity.Property(e => e.LimitBreakRewardAccessoryId).HasColumnName("limit_break_reward_accessory_id");

                entity.Property(e => e.LimitBreakRewardGiftId).HasColumnName("limit_break_reward_gift_id");

                entity.Property(e => e.LimitBreakRewardText).HasColumnName("limit_break_reward_text");

                entity.Property(e => e.MaxAgility).HasColumnName("max_agility");

                entity.Property(e => e.MaxAttack).HasColumnName("max_attack");

                entity.Property(e => e.MaxCritical).HasColumnName("max_critical");

                entity.Property(e => e.MaxHitPoint).HasColumnName("max_hit_point");

                entity.Property(e => e.MaxLevel).HasColumnName("max_level");

                entity.Property(e => e.MaxWeight).HasColumnName("max_weight");

                entity.Property(e => e.MinAgility).HasColumnName("min_agility");

                entity.Property(e => e.MinAttack).HasColumnName("min_attack");

                entity.Property(e => e.MinCritical).HasColumnName("min_critical");

                entity.Property(e => e.MinHitPoint).HasColumnName("min_hit_point");

                entity.Property(e => e.MinLevel).HasColumnName("min_level");

                entity.Property(e => e.MinWeight).HasColumnName("min_weight");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Nickname).HasColumnName("nickname");

                entity.Property(e => e.PotentialAttackArgument).HasColumnName("potential_attack_argument");

                entity.Property(e => e.PotentialGiftBorder).HasColumnName("potential_gift_border");

                entity.Property(e => e.PotentialGiftId).HasColumnName("potential_gift_id");

                entity.Property(e => e.PotentialGiftText).HasColumnName("potential_gift_text");

                entity.Property(e => e.PotentialHitPointArgument).HasColumnName("potential_hit_point_argument");

                entity.Property(e => e.Rarity).HasColumnName("rarity");

                entity.Property(e => e.Scaling).HasColumnName("scaling");

                entity.Property(e => e.SupportPoint).HasColumnName("support_point");

                entity.Property(e => e.SupportSkillId).HasColumnName("support_skill_id");
            });

            modelBuilder.Entity<CardLevel>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("card_levels");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Level).HasColumnName("level");

                entity.Property(e => e.LevelCategory).HasColumnName("level_category");

                entity.Property(e => e.MaxExp).HasColumnName("max_exp");
            });

            modelBuilder.Entity<EvolutionRecipe>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("evolution_recipes");

                entity.Property(e => e.Cost).HasColumnName("cost");

                entity.Property(e => e.GainMedal).HasColumnName("gain_medal");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Resource1Amount).HasColumnName("resource_1_amount");

                entity.Property(e => e.Resource1Id).HasColumnName("resource_1_id");

                entity.Property(e => e.Resource2Amount).HasColumnName("resource_2_amount");

                entity.Property(e => e.Resource2Id).HasColumnName("resource_2_id");

                entity.Property(e => e.Resource3Amount).HasColumnName("resource_3_amount");

                entity.Property(e => e.Resource3Id).HasColumnName("resource_3_id");
            });

            modelBuilder.Entity<SpecialAttackCard>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("special_attack_cards");

                entity.Property(e => e.BaseCardId).HasColumnName("base_card_id");

                entity.Property(e => e.ContentId).HasColumnName("content_id");

                entity.Property(e => e.ContentType).HasColumnName("content_type");

                entity.Property(e => e.EffectRate).HasColumnName("effect_rate");

                entity.Property(e => e.EndAt).HasColumnName("end_at");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PotentialEffectRate).HasColumnName("potential_effect_rate");

                entity.Property(e => e.PotentialLimit).HasColumnName("potential_limit");

                entity.Property(e => e.SpecialChapterId).HasColumnName("special_chapter_id");

                entity.Property(e => e.StartAt).HasColumnName("start_at");

                entity.Property(e => e.TestEndAt).HasColumnName("test_end_at");

                entity.Property(e => e.TestStartAt).HasColumnName("test_start_at");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
