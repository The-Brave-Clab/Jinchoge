using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class FellowRequestEntity : BaseEntity<FellowRequestEntity>
    {
        public FellowRequestEntity(
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

            var friendRequests = await player.friendRequests
                .Select(FriendRequest.Load)
                .WhenAll();
            var responseFriendRequests = await friendRequests
                .Select(Response.Data.FromFriendRequest)
                .WhenAll();
            Response responseObj = new()
            {
                fellow_requests = responseFriendRequests.ToDictionary(r => r.id, r => r)
            };
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }

        public class Response
        {
            public IDictionary<long, Data> fellow_requests { get; set; } = new Dictionary<long, Data>();

            public class Data
            {
                public long id { get; set; }
                public int status { get; set; }
                public long created_at { get; set; }
                public UserInfoEntity.Response.User from_user { get; set; } = new();

                public static async Task<Data> FromFriendRequest(FriendRequest friendRequest)
                {
                    var fromPlayerProfile = await PlayerProfile.Load(friendRequest.fromUser);
                    return new()
                    {
                        id = friendRequest.id,
                        status = friendRequest.status,
                        created_at = friendRequest.createdAt,
                        from_user = await UserInfoEntity.Response.User.FromPlayerProfile(fromPlayerProfile)
                    };
                }
            }
        }
    }
}