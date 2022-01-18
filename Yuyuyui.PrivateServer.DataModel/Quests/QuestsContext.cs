using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class QuestsContext : BaseContext<QuestsContext>
    {
        public QuestsContext()
        {
        }

        public QuestsContext(DbContextOptions<QuestsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BattleLimitEntry> BattleLimitEntries { get; set; } = null!;
        public virtual DbSet<Chapter> Chapters { get; set; } = null!;
        public virtual DbSet<ChapterReleaseCondition> ChapterReleaseConditions { get; set; } = null!;
        public virtual DbSet<Episode> Episodes { get; set; } = null!;
        public virtual DbSet<EpisodeDifficulty> EpisodeDifficulties { get; set; } = null!;
        public virtual DbSet<EpisodeReleaseCondition> EpisodeReleaseConditions { get; set; } = null!;
        public virtual DbSet<Stage> Stages { get; set; } = null!;
        public virtual DbSet<StageBattle> StageBattles { get; set; } = null!;
        public virtual DbSet<StageReleaseCondition> StageReleaseConditions { get; set; } = null!;

        protected override string DatabaseFileName => "quests";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BattleLimitEntry>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("battle_limit_entries");

                entity.Property(e => e.EnableId).HasColumnName("enable_id");

                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("chapters");

                entity.Property(e => e.BannerId).HasColumnName("banner_id");

                entity.Property(e => e.EpisodeDialogId).HasColumnName("episode_dialog_id");

                entity.Property(e => e.EpisodeNaviId).HasColumnName("episode_navi_id");

                entity.Property(e => e.EventType).HasColumnName("event_type");

                entity.Property(e => e.EventUrl).HasColumnName("event_url");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.IsEvent).HasColumnName("is_event");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.ShortName).HasColumnName("short_name");

                entity.Property(e => e.SpecialAttackLimit).HasColumnName("special_attack_limit");

                entity.Property(e => e.SpecialEvent).HasColumnName("special_event");
            });

            modelBuilder.Entity<ChapterReleaseCondition>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("chapter_release_conditions");

                entity.Property(e => e.ChapterHide).HasColumnName("chapter_hide");

                entity.Property(e => e.ChapterId).HasColumnName("chapter_id");

                entity.Property(e => e.FinishEpisodeId).HasColumnName("finish_episode_id");

                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<Episode>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("episodes");

                entity.Property(e => e.BackgroundId).HasColumnName("background_id");

                entity.Property(e => e.ChapterId).HasColumnName("chapter_id");

                entity.Property(e => e.Element).HasColumnName("element");

                entity.Property(e => e.EpisodeNaviId).HasColumnName("episode_navi_id");

                entity.Property(e => e.GoogleplayAchievementId).HasColumnName("googleplay_achievement_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.NaviMessage).HasColumnName("navi_message");

                entity.Property(e => e.Number).HasColumnName("number");

                entity.Property(e => e.StartQuestId).HasColumnName("start_quest_id");

                entity.Property(e => e.StartStageId).HasColumnName("start_stage_id");
            });

            modelBuilder.Entity<EpisodeDifficulty>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("episode_difficulties");

                entity.Property(e => e.Difficulty).HasColumnName("difficulty");

                entity.Property(e => e.EpisodeId).HasColumnName("episode_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ScoreCompletedReward1ContentId).HasColumnName("score_completed_reward_1_content_id");

                entity.Property(e => e.ScoreCompletedReward1ContentType).HasColumnName("score_completed_reward_1_content_type");

                entity.Property(e => e.ScoreCompletedReward1ItemCategoryId).HasColumnName("score_completed_reward_1_item_category_id");

                entity.Property(e => e.ScoreCompletedReward1Quantity).HasColumnName("score_completed_reward_1_quantity");

                entity.Property(e => e.ScoreCompletedReward2ContentId).HasColumnName("score_completed_reward_2_content_id");

                entity.Property(e => e.ScoreCompletedReward2ContentType).HasColumnName("score_completed_reward_2_content_type");

                entity.Property(e => e.ScoreCompletedReward2ItemCategoryId).HasColumnName("score_completed_reward_2_item_category_id");

                entity.Property(e => e.ScoreCompletedReward2Quantity).HasColumnName("score_completed_reward_2_quantity");

                entity.Property(e => e.ScoreCompletedReward3ContentId).HasColumnName("score_completed_reward_3_content_id");

                entity.Property(e => e.ScoreCompletedReward3ContentType).HasColumnName("score_completed_reward_3_content_type");

                entity.Property(e => e.ScoreCompletedReward3ItemCategoryId).HasColumnName("score_completed_reward_3_item_category_id");

                entity.Property(e => e.ScoreCompletedReward3Quantity).HasColumnName("score_completed_reward_3_quantity");
            });

            modelBuilder.Entity<EpisodeReleaseCondition>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("episode_release_conditions");

                entity.Property(e => e.EpisodeId).HasColumnName("episode_id");

                entity.Property(e => e.FinishEpisodeId).HasColumnName("finish_episode_id");

                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<Stage>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("stages");

                entity.Property(e => e.BattleBackground).HasColumnName("battle_background");

                entity.Property(e => e.BattleBgmBoss).HasColumnName("battle_bgm_boss");

                entity.Property(e => e.BattleBgmChance).HasColumnName("battle_bgm_chance");

                entity.Property(e => e.BattleBgmNormal).HasColumnName("battle_bgm_normal");

                entity.Property(e => e.BattleBgmPinch).HasColumnName("battle_bgm_pinch");

                entity.Property(e => e.BattleLimitTableId).HasColumnName("battle_limit_table_id");

                entity.Property(e => e.CardExp).HasColumnName("card_exp");

                entity.Property(e => e.ChapterId).HasColumnName("chapter_id");

                entity.Property(e => e.Difficulty).HasColumnName("difficulty");

                entity.Property(e => e.DifficultyRating).HasColumnName("difficulty_rating");

                entity.Property(e => e.Element1).HasColumnName("element_1");

                entity.Property(e => e.Element2).HasColumnName("element_2");

                entity.Property(e => e.Element3).HasColumnName("element_3");

                entity.Property(e => e.EpisodeId).HasColumnName("episode_id");

                entity.Property(e => e.ExchangePointRate).HasColumnName("exchange_point_rate");

                entity.Property(e => e.Familiarity).HasColumnName("familiarity");

                entity.Property(e => e.FinishedReward1ContentId).HasColumnName("finished_reward_1_content_id");

                entity.Property(e => e.FinishedReward1ContentType).HasColumnName("finished_reward_1_content_type");

                entity.Property(e => e.FinishedReward1ItemCategoryId).HasColumnName("finished_reward_1_item_category_id");

                entity.Property(e => e.FinishedReward1Quantity).HasColumnName("finished_reward_1_quantity");

                entity.Property(e => e.FinishedReward2ContentId).HasColumnName("finished_reward_2_content_id");

                entity.Property(e => e.FinishedReward2ContentType).HasColumnName("finished_reward_2_content_type");

                entity.Property(e => e.FinishedReward2ItemCategoryId).HasColumnName("finished_reward_2_item_category_id");

                entity.Property(e => e.FinishedReward2Quantity).HasColumnName("finished_reward_2_quantity");

                entity.Property(e => e.FinishedReward3ContentId).HasColumnName("finished_reward_3_content_id");

                entity.Property(e => e.FinishedReward3ContentType).HasColumnName("finished_reward_3_content_type");

                entity.Property(e => e.FinishedReward3ItemCategoryId).HasColumnName("finished_reward_3_item_category_id");

                entity.Property(e => e.FinishedReward3Quantity).HasColumnName("finished_reward_3_quantity");

                entity.Property(e => e.FriendPoint).HasColumnName("friend_point");

                entity.Property(e => e.GuestFriendPoint).HasColumnName("guest_friend_point");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Kind).HasColumnName("kind");

                entity.Property(e => e.Money).HasColumnName("money");

                entity.Property(e => e.NoFriend).HasColumnName("no_friend");

                entity.Property(e => e.Number).HasColumnName("number");

                entity.Property(e => e.Position).HasColumnName("position");

                entity.Property(e => e.ScenarioIdA).HasColumnName("scenario_id_a");

                entity.Property(e => e.ScenarioIdB).HasColumnName("scenario_id_b");

                entity.Property(e => e.ScoreReward1ContentId).HasColumnName("score_reward_1_content_id");

                entity.Property(e => e.ScoreReward1ContentType).HasColumnName("score_reward_1_content_type");

                entity.Property(e => e.ScoreReward1ItemCategoryId).HasColumnName("score_reward_1_item_category_id");

                entity.Property(e => e.ScoreReward1Quantity).HasColumnName("score_reward_1_quantity");

                entity.Property(e => e.ScoreReward2ContentId).HasColumnName("score_reward_2_content_id");

                entity.Property(e => e.ScoreReward2ContentType).HasColumnName("score_reward_2_content_type");

                entity.Property(e => e.ScoreReward2ItemCategoryId).HasColumnName("score_reward_2_item_category_id");

                entity.Property(e => e.ScoreReward2Quantity).HasColumnName("score_reward_2_quantity");

                entity.Property(e => e.ScoreReward3ContentId).HasColumnName("score_reward_3_content_id");

                entity.Property(e => e.ScoreReward3ContentType).HasColumnName("score_reward_3_content_type");

                entity.Property(e => e.ScoreReward3ItemCategoryId).HasColumnName("score_reward_3_item_category_id");

                entity.Property(e => e.ScoreReward3Quantity).HasColumnName("score_reward_3_quantity");

                entity.Property(e => e.StaminaUsage).HasColumnName("stamina_usage");

                entity.Property(e => e.Title).HasColumnName("title");

                entity.Property(e => e.UserExp).HasColumnName("user_exp");
            });

            modelBuilder.Entity<StageBattle>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("stage_battles");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ScoreSpeedExpeditionSecond).HasColumnName("score_speed_expedition_second");

                entity.Property(e => e.StageId).HasColumnName("stage_id");
            });

            modelBuilder.Entity<StageReleaseCondition>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("stage_release_conditions");

                entity.Property(e => e.FinishStageId).HasColumnName("finish_stage_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.StageId).HasColumnName("stage_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
