using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class AdventureBook
    {
        public long Id { get; set; }
        public string FileId { get; set; }
        public string Category { get; set; }
        public long CategoryId { get; set; }
        public string SubCategory { get; set; }
        public long SubCategoryId { get; set; }
        public string? Episode { get; set; }
        public long EpisodeId { get; set; }
        public string Name { get; set; }
        public string? ChapterName { get; set; }
        public string DisplayName { get; set; }
        public string? Label { get; set; }
        public long? AdventureBookTicketId { get; set; }
        public int? AdventureBookTicketQuantity { get; set; }
        public string? TicketUsableStartAt { get; set; } // "YYYY/M/D HH:MM:SS" (UTC) Consider Deserialize
        public string? TicketUsableEndAt { get; set; }
        public string? TestTicketUsableStartAt { get; set; }
        public string? TestTicketUsableEndAt { get; set; }
    }
}
