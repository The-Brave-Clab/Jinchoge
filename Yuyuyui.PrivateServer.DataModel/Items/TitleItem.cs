using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class TitleItem
    {
        public byte[]? Id { get; set; }
        public byte[]? Priority { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? ContentType { get; set; }
        public byte[]? Rarity { get; set; }
        public byte[]? Description { get; set; }
        public byte[]? UpgradeDescription { get; set; }
        public byte[]? OpenCondition { get; set; }
        public byte[]? OpenValues { get; set; }
        public byte[]? UpgradeRelation { get; set; }
        public byte[]? NextId { get; set; }
        public byte[]? BoardImageId { get; set; }
        public byte[]? TextImageId { get; set; }
        public byte[]? CharacterLeft { get; set; }
        public byte[]? CharacterRight { get; set; }
        public byte[]? CharacterId { get; set; }
        public byte[]? StartAt { get; set; }
        public byte[]? EndAt { get; set; }
        public byte[]? TestStartAt { get; set; }
        public byte[]? TestEndAt { get; set; }
    }
}
