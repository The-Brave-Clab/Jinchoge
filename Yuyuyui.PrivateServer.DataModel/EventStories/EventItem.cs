using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class EventItem
    {
        public byte[]? Id { get; set; }
        public byte[]? SpecialChapterId { get; set; }
        public byte[]? ExchangeBoothId { get; set; }
        public byte[]? ContentId { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? Rarity { get; set; }
        public byte[]? ImageId { get; set; }
    }
}
