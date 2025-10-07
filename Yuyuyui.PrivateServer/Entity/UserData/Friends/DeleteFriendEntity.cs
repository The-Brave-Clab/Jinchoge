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
            var player = await GetPlayerFromCookies();
            string friendCode = GetPathParameter("user_id");
            
            // Remove the friend from our friend list
            // Respects the path parameter
            player.friends.Remove(friendCode); // We don't need to check for deleting twice,
                                               // since this API is actually safe
            await player.Save();
            
            // Remove us from the friend's list
            // Respects the request body
            // This is indeed not necessary since the game does always send the same parameter
            Request request = Deserialize<Request>(requestBody)!;
            PlayerProfile friend = await PlayerProfile.Load(request.user_id);
            friend.friends.Remove(player.id.code); // Same as above
            await friend.Save();
            
            Utils.Log(string.Format(Resources.LOG_PS_FRIEND_DELETED, player.id.code, friend.id.code));
            
            responseBody = "{}"u8.ToArray();
            SetBasicResponseHeaders();
        }

        public class Request
        {
            public string user_id { get; set; } = "";
        }
    }
}