using System.Text;

namespace Yuyuyui.PrivateServer
{
    public class ProfileEntity : BaseEntity<ProfileEntity>
    {
        public ProfileEntity(
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
            bool isSession = this.GetSessionFromCookie(out var playerSession);
            if (!isSession)
            {
                throw new Exception("Session not found!");
            }

            PlayerProfile player = playerSession.player;

            if (requestBody.Length > 0)
            {
                // Doing a deserialization and then a serialization will escape the unicode characters
                RequestResponse request = Deserialize<RequestResponse>(requestBody)!;
                byte[] body = Serialize(request);
                Utils.Log($"Updated user profile:\n\tNickname\t{request.profile.nickname}\n\t Comment\t{request.profile.comment}");
                player.nickname = request.profile.nickname;
                player.comment = request.profile.comment;
                player.Save();
            }

            RequestResponse responseObj = new RequestResponse
            {
                profile = new()
                {
                    nickname = player.nickname,
                    comment = player.comment
                }
            };
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class RequestResponse
        {
            public Profile profile { get; set; } = new();

            public class Profile
            {
                public string nickname { get; set; } = "";
                public string comment { get; set; } = "";
            }
        }
    }
}