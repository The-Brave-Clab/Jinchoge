using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer
{
    public class SendFellowRequestEntity : BaseEntity<SendFellowRequestEntity>
    {
        public SendFellowRequestEntity(
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
            string friendCode = GetPathParameter("user_id");

            IList<long> playerFriendRequests;
            await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
            {
                var player = await PlayerProfile.Load(playerId.code);
                playerFriendRequests = player.friendRequests;
            }

            // We don't care about the request body anymore
            // {
            //     "user_id" = "1234567890"
            // }

            FriendRequest friendRequest;

            // Get the requested player
            // Respects the path parameter
            PlayerProfile friend;
            await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(friendCode))
            {
                // The game client checks if the friend has already been added
                // so we don't check it here.

                // However, we check if the same request have sent from the other player
                // if so, we should automatically make it accepted
                friend = await PlayerProfile.Load(friendCode);
                try
                {
                    var requests = await FriendRequest.LoadMany(playerFriendRequests);
                    friendRequest = requests.First(fr => fr.fromUser == friend.id.code);
                    Utils.Log(string.Format(Resources.LOG_PS_FRIEND_REQUEST_FOUND_SYMMETRIC,
                        friendRequest.id, friendRequest.fromUser, friendRequest.toUser));
                    friendRequest.status = 1; // Accept
                    await friendRequest.ProcessStatus();
                }
                catch (InvalidOperationException)
                {
                    friendRequest = await FriendRequest.CreateOrLoad(playerId, friend);
                }
            }

            Response responseObj = new()
            {
                fellow_request =
                    await FellowRequestEntity.Response.Data.FromFriendRequest(friendRequest)
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }

        public class Response
        {
            public FellowRequestEntity.Response.Data fellow_request { get; set; } = new();
        }
    }
}