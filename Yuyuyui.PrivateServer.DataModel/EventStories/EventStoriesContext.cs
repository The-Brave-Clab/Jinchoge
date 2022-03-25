using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class EventStoriesContext : BaseContext<EventStoriesContext>
    {
        public EventStoriesContext()
        {
        }

        public EventStoriesContext(DbContextOptions<EventStoriesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<EventItem> EventItems { get; set; } = null!;
        public virtual DbSet<SpecialAttackCharacter> SpecialAttackCharacters { get; set; } = null!;
        public virtual DbSet<SpecialChapter> SpecialChapters { get; set; } = null!;
        public virtual DbSet<SpecialEpisode> SpecialEpisodes { get; set; } = null!;
        public virtual DbSet<SpecialEpisodeCondition> SpecialEpisodeConditions { get; set; } = null!;
        public virtual DbSet<SpecialEpisodeDifficulty> SpecialEpisodeDifficulties { get; set; } = null!;
        public virtual DbSet<SpecialStage> SpecialStages { get; set; } = null!;
        public virtual DbSet<SpecialStageBattle> SpecialStageBattles { get; set; } = null!;
        public virtual DbSet<SpecialStageCondition> SpecialStageConditions { get; set; } = null!;
        public virtual DbSet<TowerEvent> TowerEvents { get; set; } = null!;
        public virtual DbSet<TowerStage> TowerStages { get; set; } = null!;
        public virtual DbSet<TowerStageReward> TowerStageRewards { get; set; } = null!;

        protected override string DatabaseFileName => "event_stories";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventItem>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("event_items");

                entity.Property(e => e.ContentId).HasColumnName("content_id");

                entity.Property(e => e.ExchangeBoothId).HasColumnName("exchange_booth_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Rarity).HasColumnName("rarity");

                entity.Property(e => e.SpecialChapterId).HasColumnName("special_chapter_id");
            });

            modelBuilder.Entity<SpecialAttackCharacter>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("special_attack_characters");

                entity.Property(e => e.CharacterId).HasColumnName("character_id");

                entity.Property(e => e.ContentId).HasColumnName("content_id");

                entity.Property(e => e.ContentType).HasColumnName("content_type");

                entity.Property(e => e.EffectRate).HasColumnName("effect_rate");

                entity.Property(e => e.EndAt).HasColumnName("end_at");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Rarity).HasColumnName("rarity");

                entity.Property(e => e.SpecialChapterId).HasColumnName("special_chapter_id");

                entity.Property(e => e.StartAt).HasColumnName("start_at");

                entity.Property(e => e.TestEndAt).HasColumnName("test_end_at");

                entity.Property(e => e.TestStartAt).HasColumnName("test_start_at");
            });

            modelBuilder.Entity<SpecialChapter>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("special_chapters");

                entity.Property(e => e.BannerId).HasColumnName("banner_id");

                entity.Property(e => e.EpisodeDialogId).HasColumnName("episode_dialog_id");

                entity.Property(e => e.EpisodeNaviId).HasColumnName("episode_navi_id");

                entity.Property(e => e.EventTabType).HasColumnName("event_tab_type");

                entity.Property(e => e.EventType).HasColumnName("event_type");

                entity.Property(e => e.EventUrl).HasColumnName("event_url");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.IsEvent).HasColumnName("is_event");

                entity.Property(e => e.LayoutId).HasColumnName("layout_id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.SanchoDays).HasColumnName("sancho_days");

                entity.Property(e => e.ShortName).HasColumnName("short_name");

                entity.Property(e => e.SpecialAttackLimit).HasColumnName("special_attack_limit");

                entity.Property(e => e.SpecialEvent).HasColumnName("special_event");
            });

            modelBuilder.Entity<SpecialEpisode>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("special_episodes");

                entity.Property(e => e.BackgroundId).HasColumnName("background_id");

                entity.Property(e => e.Element).HasColumnName("element");

                entity.Property(e => e.EpisodeNaviId).HasColumnName("episode_navi_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MapMoveCharacterId).HasColumnName("map_move_character_id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.NaviMessage).HasColumnName("navi_message");

                entity.Property(e => e.Number).HasColumnName("number");

                entity.Property(e => e.SpecialChapterId).HasColumnName("special_chapter_id");

                entity.Property(e => e.StartQuestId).HasColumnName("start_quest_id");

                entity.Property(e => e.StartSpecialStageId).HasColumnName("start_special_stage_id");
            });

            modelBuilder.Entity<SpecialEpisodeCondition>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("special_episode_conditions");

                entity.Property(e => e.EndAt).HasColumnName("end_at");

                entity.Property(e => e.FinishSpecialEpisodeId).HasColumnName("finish_special_episode_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.SpecialEpisodeId).HasColumnName("special_episode_id");

                entity.Property(e => e.StartAt).HasColumnName("start_at");
            });

            modelBuilder.Entity<SpecialEpisodeDifficulty>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("special_episode_difficulties");

                entity.Property(e => e.Difficulty).HasColumnName("difficulty");

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

                entity.Property(e => e.SpecialEpisodeId).HasColumnName("special_episode_id");
            });

            modelBuilder.Entity<SpecialStage>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("special_stages");

                entity.Property(e => e.AutoClear).HasColumnName("auto_clear");

                entity.Property(e => e.BattleBackground).HasColumnName("battle_background");

                entity.Property(e => e.BattleBgmBoss).HasColumnName("battle_bgm_boss");

                entity.Property(e => e.BattleBgmChance).HasColumnName("battle_bgm_chance");

                entity.Property(e => e.BattleBgmNormal).HasColumnName("battle_bgm_normal");

                entity.Property(e => e.BattleBgmPinch).HasColumnName("battle_bgm_pinch");

                entity.Property(e => e.BattleLimitTableId).HasColumnName("battle_limit_table_id");

                entity.Property(e => e.CardExp).HasColumnName("card_exp");

                entity.Property(e => e.Difficulty).HasColumnName("difficulty");

                entity.Property(e => e.DifficultyRating).HasColumnName("difficulty_rating");

                entity.Property(e => e.Element1).HasColumnName("element_1");

                entity.Property(e => e.Element2).HasColumnName("element_2");

                entity.Property(e => e.Element3).HasColumnName("element_3");

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

                entity.Property(e => e.SpecialChapterId).HasColumnName("special_chapter_id");

                entity.Property(e => e.SpecialEpisodeId).HasColumnName("special_episode_id");

                entity.Property(e => e.StaminaUsage).HasColumnName("stamina_usage");

                entity.Property(e => e.Title).HasColumnName("title");

                entity.Property(e => e.UserExp).HasColumnName("user_exp");

                entity.Property(e => e.WeeklyScore).HasColumnName("weekly_score");
            });

            modelBuilder.Entity<SpecialStageBattle>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("special_stage_battles");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ScoreSpeedExpeditionSecond).HasColumnName("score_speed_expedition_second");

                entity.Property(e => e.SpecialStageId).HasColumnName("special_stage_id");
            });

            modelBuilder.Entity<SpecialStageCondition>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("special_stage_conditions");

                entity.Property(e => e.FinishSpecialStageId).HasColumnName("finish_special_stage_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.SpecialStageId).HasColumnName("special_stage_id");
            });

            modelBuilder.Entity<TowerEvent>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("tower_events");

                entity.Property(e => e.Act).HasColumnName("act");

                entity.Property(e => e.AvailableUserCardLevel).HasColumnName("available_user_card_level");

                entity.Property(e => e.AvailableUserLevel).HasColumnName("available_user_level");

                entity.Property(e => e.EndAt).HasColumnName("end_at");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.SecondsForRecoverUserCardsFromStartOfDay).HasColumnName("seconds_for_recover_user_cards_from_start_of_day");

                entity.Property(e => e.SpecialChapterId).HasColumnName("special_chapter_id");

                entity.Property(e => e.StartAt).HasColumnName("start_at");

                entity.Property(e => e.TestEndAt).HasColumnName("test_end_at");

                entity.Property(e => e.TestStartEndAt).HasColumnName("test_start_end_at");

                entity.Property(e => e.UsableCountOfAUserCardPerDay).HasColumnName("usable_count_of_a_user_card_per_day");
            });

            modelBuilder.Entity<TowerStage>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("tower_stages");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Level).HasColumnName("level");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.SpecialStageId).HasColumnName("special_stage_id");

                entity.Property(e => e.TowerEventId).HasColumnName("tower_event_id");
            });

            modelBuilder.Entity<TowerStageReward>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("tower_stage_rewards");

                entity.Property(e => e.ContentId).HasColumnName("content_id");

                entity.Property(e => e.ContentType).HasColumnName("content_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.TowerStageId).HasColumnName("tower_stage_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
