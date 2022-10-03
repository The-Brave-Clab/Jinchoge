using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class SendFellowRequestEntity : BaseEntity<SendFellowRequestEntity>
    {
        public SendFellowRequestEntity(
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
            string friendCode = GetPathParameter("user_id");

            // Get the requested player
            // Respects the path parameter
            var friend = PlayerProfile.Load(friendCode);

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
                friendRequest =
                    player.friendRequests
                        .Select(FriendRequest.Load)
                        .First(fr => fr.fromUser == friend.id.code);
                Utils.Log(
                    $"Found symmetric friend request {friendRequest.id} ({friendRequest.fromUser}=>{friendRequest.toUser})!");
                friendRequest.status = 1; // Accept
                friendRequest.ProcessStatus();
            }
            catch (InvalidOperationException)
            {
                friendRequest = FriendRequest.CreateOrLoad(player, friend);

                if (friend.IsConfigPlayer())
                {
                    Utils.Log($"Player sent friend request to a config player <{friend.profile.comment}>");
                    friendRequest.status = 1; // Accept
                    friendRequest.ProcessStatus();
                }
            }

            Response responseObj;
            using (var cardsDb = new CardsContext())
            using (var charactersDb = new CharactersContext())
            {
                {responseObj = new()
                {
                    fellow_request =
                        FellowRequestEntity.Response.Data.FromFriendRequest(cardsDb, charactersDb, friendRequest)
                };}
            }

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public FellowRequestEntity.Response.Data fellow_request { get; set; } = new();
        }
    }
}