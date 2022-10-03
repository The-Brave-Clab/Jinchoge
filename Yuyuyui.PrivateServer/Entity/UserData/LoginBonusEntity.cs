using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class LoginBonusEntity : BaseEntity<LoginBonusEntity>
    {
        public LoginBonusEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();

            // Looks like the request body is always "{}". Ignored for now.
            Utils.LogWarning("Stub API! Returns nothing for now.");

            Response responseObj = new()
            {
                login_bonuses = new List<Response.LoginBonus>()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IList<LoginBonus> login_bonuses { get; set; } = new List<LoginBonus>();

            public class LoginBonus
            {
                public long master_id { get; set; }
                public string name { get; set; } = "";
                public IList<BonusItem> items { get; set; } = new List<BonusItem>();
                public int bonus_category_id { get; set; } // ?
                public int character_id { get; set; }
                public string first_message { get; set; } = "";
                public string first_message_voice_id { get; set; } = "";
                public string second_message { get; set; } = "";
                public string second_message_voice_id { get; set; } = "";
                public int? background_id { get; set; } = null;
                public int omikuji_result { get; set; } // ?
            }

            public class BonusItem
            {
                public int item_category_id { get; set; }
                public int amount { get; set; }
                public long master_id { get; set; }
                public int aquired_status { get; set; } // 2: acquired, 1: today, 0: not acquired (what a typo)
                public int? days { get; set; } = null; // ?
            }
        }
    }
}