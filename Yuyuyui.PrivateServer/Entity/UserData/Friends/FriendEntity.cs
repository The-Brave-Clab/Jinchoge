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
            var playerId = await GetPlayerIdFromCookies();

            IList<string> playerFriends;
            await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
            {
                var player = await PlayerProfile.Load(playerId.code);
                playerFriends = player.friends;
            }

            var friends = await PlayerProfile.LoadMany(playerFriends);
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