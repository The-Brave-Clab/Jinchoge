using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();
            string friendCode = GetPathParameter("user_id");
            
            // Remove the friend from our friend list
            // Respects the path parameter
            player.friends.Remove(friendCode); // We don't need to check for deleting twice,
                                               // since this API is actually safe
            player.Save();
            
            // Remove us from the friend's list
            // Respects the request body
            // This is indeed not necessary since the game does always send the same parameter
            Request request = Deserialize<Request>(requestBody)!;
            PlayerProfile friend = PlayerProfile.Load(request.user_id);
            friend.friends.Remove(player.id.code); // Same as above
            friend.Save();
            
            Utils.Log($"Player {player.id.code} removed player {friend.id.code} from their friend list.");
            
            responseBody = Encoding.UTF8.GetBytes("{}");
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Request
        {
            public string user_id { get; set; } = "";
        }
    }
}