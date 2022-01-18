using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class Accessory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Rarity { get; set; }
        public int Element { get; set; }
        public int MaxLevel { get; set; }
        public int MinCost { get; set; }
        public int MaxCost { get; set; }
        public int MinHitPoint { get; set; }
        public int MaxHitPoint { get; set; }
        public int MinAttack { get; set; }
        public int MaxAttack { get; set; }
        public long SkillId { get; set; }
        public int UniqueId { get; set; }
    }
}
