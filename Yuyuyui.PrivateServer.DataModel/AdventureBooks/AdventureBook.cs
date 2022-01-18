using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class AdventureBook
    {
        public byte[]? Id { get; set; }
        public byte[]? FileId { get; set; }
        public byte[]? Category { get; set; }
        public byte[]? CategoryId { get; set; }
        public byte[]? SubCategory { get; set; }
        public byte[]? SubCategoryId { get; set; }
        public byte[]? Episode { get; set; }
        public byte[]? EpisodeId { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? ChapterName { get; set; }
        public byte[]? DisplayName { get; set; }
        public byte[]? Label { get; set; }
        public byte[]? AdventureBookTicketId { get; set; }
        public byte[]? AdventureBookTicketQuantity { get; set; }
        public byte[]? TicketUsableStartAt { get; set; }
        public byte[]? TicketUsableEndAt { get; set; }
        public byte[]? TestTicketUsableStartAt { get; set; }
        public byte[]? TestTicketUsableEndAt { get; set; }
    }
}
