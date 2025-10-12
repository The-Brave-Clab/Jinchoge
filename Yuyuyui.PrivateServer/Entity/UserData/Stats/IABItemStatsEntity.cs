using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class IABItemStatsEntity : BaseEntity<IABItemStatsEntity>
    {
        public IABItemStatsEntity(
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

            PlayerProfile.Data playerData;
            await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
            {
                var player = await PlayerProfile.Load(playerId.code);
                playerData = player.data;
            }

            // Utils.LogWarning("Fixed number of 1,000,000 paid blessings");

            Response responseObj = new()
            {
                paid_point = playerData.paidBlessing,
                free_point = playerData.freeBlessing
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }

        public class Response
        {
            public int paid_point { get; set; }
            public int free_point { get; set; }
        }
    }
}