using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Gacha
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Kind { get; set; }
        public long? StepupGroup { get; set; } // Is this id?
        public int? StepupOrder { get; set; }
        public int? StepupLoop { get; set; } // boolean, 1 for true, null for false
        public int? SpecialGet { get; set; } // boolean, 1 for true, null for false
        public int? SpecialGetCount { get; set; }
        public int? SpecialSaveRarity { get; set; }
        public int? SelectGacha { get; set; } // boolean, 1 for true, null for false
        public int? SelectCount { get; set; }
        public int? SpecialSelect { get; set; } // boolean, 1 for true, null for false
        public int? MinUserLevel { get; set; }
        public int? MaxUserLevel { get; set; }
        public int? CountDownGacha { get; set; } // boolean, 1 for true, null for false
        public int? NoTicketDisplay { get; set; } // boolean, 1 for true, null for false
        public int? NoDisplayEndAt { get; set; } // boolean, 1 for true, null for false
        public string StartAt { get; set; }
        public string EndAt { get; set; }
        public string TestStartAt { get; set; }
        public string TestEndAt { get; set; }
        public long? GachaTopicId { get; set; } // All null
        public string? PickupType { get; set; } // "<string>;<string>;<string>;<string>...<string>"
        public string? PickupId { get; set; } // "<long>;<long>;<long>...<long>"
        public int? SkipType { get; set; } // 01 boolean
        public string? PopupSeName { get; set; }
    }
}
