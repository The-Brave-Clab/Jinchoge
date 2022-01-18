using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Gacha
    {
        public byte[]? Id { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? Description { get; set; }
        public byte[]? Kind { get; set; }
        public byte[]? StepupGroup { get; set; }
        public byte[]? StepupOrder { get; set; }
        public byte[]? StepupLoop { get; set; }
        public byte[]? SpecialGet { get; set; }
        public byte[]? SpecialGetCount { get; set; }
        public byte[]? SpecialSaveRarity { get; set; }
        public byte[]? SelectGacha { get; set; }
        public byte[]? SelectCount { get; set; }
        public byte[]? SpecialSelect { get; set; }
        public byte[]? MinUserLevel { get; set; }
        public byte[]? MaxUserLevel { get; set; }
        public byte[]? CountDownGacha { get; set; }
        public byte[]? NoTicketDisplay { get; set; }
        public byte[]? NoDisplayEndAt { get; set; }
        public byte[]? StartAt { get; set; }
        public byte[]? EndAt { get; set; }
        public byte[]? TestStartAt { get; set; }
        public byte[]? TestEndAt { get; set; }
        public byte[]? GachaTopicId { get; set; }
        public byte[]? PickupType { get; set; }
        public byte[]? PickupId { get; set; }
        public byte[]? SkipType { get; set; }
        public byte[]? PopupSeName { get; set; }
    }
}
