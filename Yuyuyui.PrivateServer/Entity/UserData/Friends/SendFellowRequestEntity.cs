using System.Text;

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
            
            // The game client checks if the friend has already been added
            // so we don't check it here.
            
            // We don't care about the request body anymore
            // {
            //     "user_id" = "1234567890"
            // }

            var friendRequest = FriendRequest.CreateOrLoad(player, friend);

            Response responseObj = new()
            {
                fellow_request = friendRequest
            };
            
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