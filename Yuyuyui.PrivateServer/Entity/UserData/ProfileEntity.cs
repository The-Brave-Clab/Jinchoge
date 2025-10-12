using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer
{
    public class ProfileEntity : BaseEntity<ProfileEntity>
    {
        public ProfileEntity(
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

            PlayerProfile.Profile playerProfile;
            await using (await IDistributedLockProvider.ActiveProvider!.AcquirePlayerProfileLock(playerId.code))
            {
                var player = await PlayerProfile.Load(playerId.code);

                if (requestBody.Length > 0)
                {
                    RequestResponse request = Deserialize<RequestResponse>(requestBody)!;
                    Utils.Log(string.Format(Resources.LOG_PS_PROFILE_UPDATED,
                        request.profile.nickname, request.profile.comment));
                    player.profile = request.profile;
                    await player.Save();
                }

                playerProfile = player.profile;
            }

            RequestResponse responseObj = new RequestResponse
            {
                profile = playerProfile
            };
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }

        public class RequestResponse
        {
            public PlayerProfile.Profile profile { get; set; } = new();
        }
    }
}