using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class LoginBonusSheet
    {
        public byte[]? Id { get; set; }
        public byte[]? Kind { get; set; }
        public byte[]? MaxProgress { get; set; }
        public byte[]? StartAt { get; set; }
        public byte[]? EndAt { get; set; }
        public byte[]? TestStartAt { get; set; }
        public byte[]? TestEndAt { get; set; }
        public byte[]? NextSheetId { get; set; }
        public byte[]? ComebackDate { get; set; }
    }
}
