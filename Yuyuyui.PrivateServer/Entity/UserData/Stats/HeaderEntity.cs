using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer;

public class HeaderEntity : BaseEntity<HeaderEntity>
{
    public HeaderEntity(
        Uri requestUri,
        string httpMethod,
        Dictionary<string, string> requestHeaders,
        byte[] requestBody,
        RouteConfig config)
        : base(requestUri, httpMethod, requestHeaders, requestBody, config)
    {
    }

    protected override async Task ProcessRequest()
    {
        var playerId = await GetPlayerIdFromCookies();

        bool infiniteItems;
        PlayerProfile.Data playerData;
        await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
        {
            var player = await PlayerProfile.Load(playerId.code);
            infiniteItems = await PrivateServer.ResourceProvider.inGameConfigProvider.GetInfiniteItems(player);
            playerData = player.data;
        }

        // Utils.LogWarning("Many data is stub");


        Response responseObj = new()
        {
            is_sancho = false, // We all know this
            header = new()
            {
                level = playerData.level,
                exp = playerData.exp,
                next_level_exp = 110, // database
                title_item_id = playerData.titleItemID,
                is_level_effect = 0, // what is this
                stamina = playerData.stamina,
                max_stamina = 140, // database
                exceeded_stamina = 0, // do the math!
                stamina_full_recover_at = 0, // unixtime
                stamina_recovery_second = 300, // fixed?
                money = infiniteItems ? 99999999 : playerData.money,
                friend_point = infiniteItems ? 99999999 : playerData.friendPoint,
                billing_point = infiniteItems ? 999999 : playerData.paidBlessing + playerData.freeBlessing,
                brave_coin = infiniteItems ? 999 : playerData.braveCoin,
                enhancement_item_capacity = 590, // database + player bought
                has_complete_mission = false, // club order prompt
                has_present = false,
                weekday_stamina = playerData.weekdayStamina,
                max_weekday_stamina = 6, // brave system?
                exceeded_weekday_stamina = 0, // do the math!
                weekday_stamina_full_recover_at = 0, // unixtime
                weekday_stamina_recovery_second = 3600, // fixed?
                exchange_point = infiniteItems ? 99999999 : playerData.exchangePoint,
                exchange_point_capacity = 99999999 // fixed?
            }
                
        };

        responseBody = Serialize(responseObj);
        SetBasicResponseHeaders();
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