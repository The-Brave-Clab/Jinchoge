using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class FriendEntity : BaseEntity<FriendEntity>
    {
        public FriendEntity(
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

            Response responseObj = new()
            {
                fellowships = player.friends
                    .Select(PlayerProfile.Load)
                    .ToDictionary(p => p.id.code, UserInfoEntity.Response.User.FromPlayerProfile)
            };
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IDictionary<string, UserInfoEntity.Response.User> fellowships { get; set; } = 
                new Dictionary<string, UserInfoEntity.Response.User>();
        }
    }
}