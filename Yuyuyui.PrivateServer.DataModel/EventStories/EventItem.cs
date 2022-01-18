using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class EventItem
    {
        public long Id { get; set; }
        public long SpecialChapterId { get; set; }
        public long ExchangeBoothId { get; set; }
        public long ContentId { get; set; }
        public string Name { get; set; } = "";
        public int Rarity { get; set; }
        public long ImageId { get; set; }
    }
}
