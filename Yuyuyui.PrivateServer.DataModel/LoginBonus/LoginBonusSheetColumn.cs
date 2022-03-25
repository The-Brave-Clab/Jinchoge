using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class LoginBonusSheetColumn
    {
        public long Id { get; set; }
        public long LoginBonusSheetId { get; set; }
        public int Progress { get; set; }
        public long GiftId { get; set; }
    }
}
