using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class TowerStage
    {
        public byte[]? Id { get; set; }
        public byte[]? TowerEventId { get; set; }
        public byte[]? Name { get; set; }
        public byte[]? Level { get; set; }
        public byte[]? SpecialStageId { get; set; }
    }
}
