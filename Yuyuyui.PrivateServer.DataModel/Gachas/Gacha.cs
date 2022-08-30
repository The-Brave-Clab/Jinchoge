using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Gacha
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int Kind { get; set; }
        public long? StepupGroup { get; set; } = null; // Is this id?
        public int? StepupOrder { get; set; } = null;
        public int? StepupLoop { get; set; } = null; // boolean, 1 for true, null for false
        public int? SpecialGet { get; set; } = null; // boolean, 1 for true, null for false
        public int? SpecialGetCount { get; set; } = null;
        public int? SpecialSaveRarity { get; set; } = null;
        public string? SelectGacha { get; set; } = null; // boolean, 1 for true, null for false
        public int? SelectCount { get; set; } = null;
        public string? SpecialSelect { get; set; } = null; // boolean, 1 for true, null for false
        public int? MinUserLevel { get; set; } = null;
        public int? MaxUserLevel { get; set; } = null;
        public int? CountDownGacha { get; set; } = null; // boolean, 1 for true, null for false
        public int? NoTicketDisplay { get; set; } = null; // boolean, 1 for true, null for false
        public string? NoDisplayEndAt { get; set; } = null; // boolean, 1 for true, null for false
        public string StartAt { get; set; } = "";
        public string EndAt { get; set; } = "";
        public string TestStartAt { get; set; } = "";
        public string TestEndAt { get; set; } = "";
        public long? GachaTopicId { get; set; } = null; // All null
        public string? PickupType { get; set; } = null; // "<string>;<string>;<string>;<string>...<string>"
        public string? PickupId { get; set; } = null; // "<long>;<long>;<long>...<long>"
        public int? SkipType { get; set; } = null; // 01 boolean
        public string? PopupSeName { get; set; } = null;
    }
}