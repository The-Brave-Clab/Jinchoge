using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Chapter
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string ShortName { get; set; } = "";
        public long ImageId { get; set; }
        public long? BannerId { get; set; } = null; // All null
        public string IsEvent { get; set; } = ""; // boolean, All FALSE
        public int? EventType { get; set; } = null; // All null
        public string? EventUrl { get; set; } = null; // All null
        public string EpisodeNaviId { get; set; } = ""; // <long>,<long>,<long>, character ids
        public string EpisodeDialogId { get; set; } = ""; // "titleCallAll" and "titleCall"
        public string? SpecialEvent { get; set; } = null; // All null, data type unknown (maybe int)
        public int? SpecialAttackLimit { get; set; } = null; // All null
    }
}