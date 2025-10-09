using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class FriendEntity : BaseEntity<FriendEntity>
    {
        public FriendEntity(
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
            var player = await GetPlayerFromCookies();

            var friends = await PlayerProfile.LoadMany(player.friends);
            var responseFromPlayerProfile = await friends
                .Select(UserInfoEntity.Response.User.FromPlayerProfile)
                .WhenAll();
            Response responseObj = new()
            {
                fellowships = responseFromPlayerProfile.ToDictionary(p => p.id, p => p)
            };
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }

        public class Response
        {
            public IDictionary<string, UserInfoEntity.Response.User> fellowships { get; set; } = 
                new Dictionary<string, UserInfoEntity.Response.User>();
        }
    }
}