using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Accessory
    {
        public byte[]? Id { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? Rarity { get; set; }
        public byte[]? Element { get; set; }
        public byte[]? MaxLevel { get; set; }
        public byte[]? MinCost { get; set; }
        public byte[]? MaxCost { get; set; }
        public byte[]? MinHitPoint { get; set; }
        public byte[]? MaxHitPoint { get; set; }
        public byte[]? MinAttack { get; set; }
        public byte[]? MaxAttack { get; set; }
        public byte[]? SkillId { get; set; }
        public byte[]? UniqueId { get; set; }
    }
}
