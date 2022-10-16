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

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();

            if (requestBody.Length > 0)
            {
                RequestResponse request = Deserialize<RequestResponse>(requestBody)!;
                Utils.Log(string.Format(Resources.LOG_PS_PROFILE_UPDATED,
                    request.profile.nickname, request.profile.comment));
                player.profile = request.profile;
                player.Save();
            }

            RequestResponse responseObj = new RequestResponse
            {
                profile = player.profile
            };
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class RequestResponse
        {
            public PlayerProfile.Profile profile { get; set; } = new();
        }
    }
}