using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Chapter
    {
        public byte[]? Id { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? ShortName { get; set; }
        public byte[]? ImageId { get; set; }
        public byte[]? BannerId { get; set; }
        public byte[]? IsEvent { get; set; }
        public byte[]? EventType { get; set; }
        public byte[]? EventUrl { get; set; }
        public byte[]? EpisodeNaviId { get; set; }
        public byte[]? EpisodeDialogId { get; set; }
        public byte[]? SpecialEvent { get; set; }
        public byte[]? SpecialAttackLimit { get; set; }
    }
}
