namespace Yuyuyui.PrivateServer
{
    public class HeaderEntity : BaseEntity<HeaderEntity>
    {
        public HeaderEntity(
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

            Utils.LogWarning("Many data is stub");

            Response responseObj = new()
            {
                is_sancho = false, // We all know this
                header = new()
                {
                    level = player.data.level,
                    exp = player.data.exp,
                    next_level_exp = 110, // database
                    title_item_id = player.data.titleItemID,
                    is_level_effect = 0, // what is this
                    stamina = player.data.stamina,
                    max_stamina = 140, // database
                    exceeded_stamina = 0, // do the math!
                    stamina_full_recover_at = 0, // unixtime
                    stamina_recovery_second = 300, // fixed?
                    money = player.data.money,
                    friend_point = player.data.friendPoint,
                    billing_point = player.data.paidBlessing + player.data.freeBlessing,
                    brave_coin = player.data.braveCoin,
                    enhancement_item_capacity = 590, // database + player bought
                    has_complete_mission = false, // ?
                    has_present = false,
                    weekday_stamina = player.data.weekdayStamina,
                    max_weekday_stamina = 6, // brave system?
                    exceeded_weekday_stamina = 0, // do the math!
                    weekday_stamina_full_recover_at = 0, // unixtime
                    weekday_stamina_recovery_second = 3600, // fixed?
                    exchange_point = player.data.exchangePoint,
                    exchange_point_capacity = 99999999 // fixed?
                }
                
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public bool is_sancho { get; set; }
            public Header header { get; set; } = new();

            public class Header
            {
                public int level { get; set; }
                public long exp { get; set; }
                public long next_level_exp { get; set; } // user_levels.db/user_levels/max_exp + 1
                public long? title_item_id { get; set; } = null;
                public int is_level_effect { get; set; } // ? looks like 0, 1 (maybe enum)
                public int stamina { get; set; }
                public int max_stamina { get; set; } // user_levels.d b
                public int exceeded_stamina { get; set; }
                public long stamina_full_recover_at { get; set; } // unixtime, 0 if not applied
                public int stamina_recovery_second { get; set; } // looks like fixed 300?
                public long money { get; set; }
                public int friend_point { get; set; }
                public int billing_point { get; set; }
                public int brave_coin { get; set; }
                public int enhancement_item_capacity { get; set; }
                public bool has_complete_mission { get; set; }
                public bool has_present { get; set; }
                public int weekday_stamina { get; set; }
                public int max_weekday_stamina { get; set; }
                public int exceeded_weekday_stamina { get; set; }
                public long weekday_stamina_full_recover_at { get; set; }
                public int weekday_stamina_recovery_second { get; set; }
                public int exchange_point { get; set; } // Taisha Point
                public int exchange_point_capacity { get; set; } // fixed 99999999?
            }
        }
    }
}