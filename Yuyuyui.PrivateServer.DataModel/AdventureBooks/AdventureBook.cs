using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class AdventureBook
    {
        public long Id { get; set; }
        public string FileId { get; set; } = "";
        public string Category { get; set; } = "";
        public long CategoryId { get; set; }
        public string SubCategory { get; set; } = "";
        public long SubCategoryId { get; set; }
        public string? Episode { get; set; } = null;
        public long EpisodeId { get; set; }
        public string Name { get; set; } = "";
        public string? ChapterName { get; set; } = null;
        public string DisplayName { get; set; } = "";
        public string? Label { get; set; } = null;
        public long? AdventureBookTicketId { get; set; } = null;
        public int? AdventureBookTicketQuantity { get; set; } = null;
        public string? TicketUsableStartAt { get; set; } = null; // "YYYY/M/D HH:MM:SS" (UTC) Consider Deserialize
        public string? TicketUsableEndAt { get; set; } = null;
        public string? TestTicketUsableStartAt { get; set; } = null;
        public string? TestTicketUsableEndAt { get; set; } = null;
    }
}
