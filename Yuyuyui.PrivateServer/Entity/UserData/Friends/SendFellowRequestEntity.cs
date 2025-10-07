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
            var player = await GetPlayerFromCookies();
            string friendCode = GetPathParameter("user_id");

            // Get the requested player
            // Respects the path parameter
            var friend = await PlayerProfile.Load(friendCode);

            // We don't care about the request body anymore
            // {
            //     "user_id" = "1234567890"
            // }

            FriendRequest friendRequest;

            // The game client checks if the friend has already been added
            // so we don't check it here.

            // However, we check if the same request have sent from the other player
            // if so, we should automatically make it accepted
            try
            {
                var requests = await player.friendRequests
                    .Select(FriendRequest.Load)
                    .WhenAll();
                friendRequest = requests.First(fr => fr.fromUser == friend.id.code);
                Utils.Log(string.Format(Resources.LOG_PS_FRIEND_REQUEST_FOUND_SYMMETRIC,
                    friendRequest.id, friendRequest.fromUser, friendRequest.toUser));
                friendRequest.status = 1; // Accept
                await friendRequest.ProcessStatus();
            }
            catch (InvalidOperationException)
            {
                friendRequest = await FriendRequest.CreateOrLoad(player, friend);
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