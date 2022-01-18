using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class LoginBonusSheet
    {
        public long Id { get; set; }
        public int Kind { get; set; }
        public int MaxProgress { get; set; }
        public string StartAt { get; set; } = "";
        public string EndAt { get; set; } = "";
        public string TestStartAt { get; set; } = "";
        public string TestEndAt { get; set; } = "";
        public long? NextSheetId { get; set; } = null;
        public string? ComebackDate { get; set; } = null; // Date in m/d/yy
    }
}
