using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SpecialChapter
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string ShortName { get; set; } = "";
        public long ImageId { get; set; }
        public long BannerId { get; set; }
        public string IsEvent { get; set; } = ""; // boolean, All TRUE
        public int EventType { get; set; }
        public long? LayoutId { get; set; } = null;
        public int Priority { get; set; }
        public int? SpecialEvent { get; set; } = null; // boolean, 1 for true, null for false
        public int? SpecialAttackLimit { get; set; } = null;
        public int EventTabType { get; set; }
        public string? EventUrl { get; set; } = null;
        public long? EpisodeNaviId { get; set; } = null; // All null
        public long? EpisodeDialogId { get; set; } = null; // All null
        public int SanchoDays { get; set; } // 01 boolean
    }
}
