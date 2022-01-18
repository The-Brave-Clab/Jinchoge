using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class CartoonsContext : BaseContext<CartoonsContext>
    {
        public CartoonsContext()
        {
        }

        public CartoonsContext(DbContextOptions<CartoonsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BingoReward> BingoRewards { get; set; } = null!;
        public virtual DbSet<BingoSheet> BingoSheets { get; set; } = null!;
        public virtual DbSet<BingoSquare> BingoSquares { get; set; } = null!;
        public virtual DbSet<CartoonChapter> CartoonChapters { get; set; } = null!;
        public virtual DbSet<CartoonFrame> CartoonFrames { get; set; } = null!;
        public virtual DbSet<CartoonStory> CartoonStories { get; set; } = null!;

        protected override string DatabaseFileName => "cartoons";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BingoReward>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("bingo_rewards");

                entity.Property(e => e.ContentId).HasColumnName("content_id");

                entity.Property(e => e.ContentType).HasColumnName("content_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.Title).HasColumnName("title");
            });

            modelBuilder.Entity<BingoSheet>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("bingo_sheets");

                entity.Property(e => e.BingoRewardId).HasColumnName("bingo_reward_id");

                entity.Property(e => e.CartoonChapterId).HasColumnName("cartoon_chapter_id");

                entity.Property(e => e.EndAt).HasColumnName("end_at");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.StartAt).HasColumnName("start_at");

                entity.Property(e => e.TestEndAt).HasColumnName("test_end_at");

                entity.Property(e => e.TestStartAt).HasColumnName("test_start_at");
            });

            modelBuilder.Entity<BingoSquare>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("bingo_squares");

                entity.Property(e => e.BingoOpenItemId).HasColumnName("bingo_open_item_id");

                entity.Property(e => e.BingoOpenItemNum).HasColumnName("bingo_open_item_num");

                entity.Property(e => e.BingoRewardId).HasColumnName("bingo_reward_id");

                entity.Property(e => e.BingoSheetId).HasColumnName("bingo_sheet_id");

                entity.Property(e => e.CartoonFrameId).HasColumnName("cartoon_frame_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Position).HasColumnName("position");
            });

            modelBuilder.Entity<CartoonChapter>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("cartoon_chapters");

                entity.Property(e => e.ChapterNum).HasColumnName("chapter_num");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Title).HasColumnName("title");
            });

            modelBuilder.Entity<CartoonFrame>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("cartoon_frames");

                entity.Property(e => e.CartoonStoryId).HasColumnName("cartoon_story_id");

                entity.Property(e => e.FrameNum).HasColumnName("frame_num");

                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<CartoonStory>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("cartoon_stories");

                entity.Property(e => e.CartoonChapterId).HasColumnName("cartoon_chapter_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.StoryNum).HasColumnName("story_num");

                entity.Property(e => e.Title).HasColumnName("title");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
