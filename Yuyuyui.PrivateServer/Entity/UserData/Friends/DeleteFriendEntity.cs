using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer
{
    public class DeleteFriendEntity : BaseEntity<DeleteFriendEntity>
    {
        public DeleteFriendEntity(
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
            
            // Remove the friend from our friend list
            // Respects the path parameter
            await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
            {
                PlayerProfile player = await PlayerProfile.Load(playerId.code);
                if (player.friends.Remove(friendCode))
                    await player.Save();
            }
            
            // Remove us from the friend's list
            // Respects the request body
            // This is indeed not necessary since the game does always send the same parameter
            Request request = Deserialize<Request>(requestBody)!;
            await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(request.user_id))
            {
                PlayerProfile friend = await PlayerProfile.Load(request.user_id);
                if (friend.friends.Remove(playerId.code))
                    await friend.Save();
            }

            Utils.Log(string.Format(Resources.LOG_PS_FRIEND_DELETED, playerId.code, request.user_id));
            
            responseBody = "{}"u8.ToArray();
            SetBasicResponseHeaders();
        }

        public class Request
        {
            public string user_id { get; set; } = "";
        }
    }
}