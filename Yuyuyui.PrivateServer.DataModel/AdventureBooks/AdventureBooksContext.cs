using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class AdventureBooksContext : BaseContext<AdventureBooksContext>
    {
        public AdventureBooksContext()
        {
        }

        public AdventureBooksContext(DbContextOptions<AdventureBooksContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AdventureBook> AdventureBooks { get; set; } = null!;
        public virtual DbSet<AdventureBookTicket> AdventureBookTickets { get; set; } = null!;
        public virtual DbSet<BundleAdventureBook> BundleAdventureBooks { get; set; } = null!;

        protected override string DatabaseFileName => "adventure_books";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdventureBook>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("adventure_books");

                entity.Property(e => e.AdventureBookTicketId).HasColumnName("adventure_book_ticket_id");

                entity.Property(e => e.AdventureBookTicketQuantity).HasColumnName("adventure_book_ticket_quantity");

                entity.Property(e => e.Category).HasColumnName("category");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.ChapterName).HasColumnName("chapter_name");

                entity.Property(e => e.DisplayName).HasColumnName("display_name");

                entity.Property(e => e.Episode).HasColumnName("episode");

                entity.Property(e => e.EpisodeId).HasColumnName("episode_id");

                entity.Property(e => e.FileId).HasColumnName("file_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Label).HasColumnName("label");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.SubCategory).HasColumnName("sub_category");

                entity.Property(e => e.SubCategoryId).HasColumnName("sub_category_id");

                entity.Property(e => e.TestTicketUsableEndAt).HasColumnName("test_ticket_usable_end_at");

                entity.Property(e => e.TestTicketUsableStartAt).HasColumnName("test_ticket_usable_start_at");

                entity.Property(e => e.TicketUsableEndAt).HasColumnName("ticket_usable_end_at");

                entity.Property(e => e.TicketUsableStartAt).HasColumnName("ticket_usable_start_at");
            });

            modelBuilder.Entity<AdventureBookTicket>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("adventure_book_tickets");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<BundleAdventureBook>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("bundle_adventure_books");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
