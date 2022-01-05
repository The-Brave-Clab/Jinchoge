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
            var player = GetPlayerFromCookies();

            if (requestBody.Length > 0)
            {
                RequestResponse request = Deserialize<RequestResponse>(requestBody)!;
                Utils.Log($"Updated user profile:\n\tNickname\t{request.profile.nickname}\n\t Comment\t{request.profile.comment}");
                player.profile = request.profile;
                player.Save();
            }

            RequestResponse responseObj = new RequestResponse
            {
                profile = player.profile
            };
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class RequestResponse
        {
            public PlayerProfile.Profile profile { get; set; } = new();
        }
    }
}