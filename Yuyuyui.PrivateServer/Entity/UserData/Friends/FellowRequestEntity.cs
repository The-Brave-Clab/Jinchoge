namespace Yuyuyui.PrivateServer
{
    public class FellowRequestEntity : BaseEntity<FellowRequestEntity>
    {
        public FellowRequestEntity(
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

            Response responseObj = new()
            {
                fellow_requests = player.friendRequests
                    .Select(FriendRequest.Load)
                    .ToDictionary(fr => fr.id, Response.Data.FromFriendRequest)
            };
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
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

                public static Data FromFriendRequest(FriendRequest friendRequest)
                {
                    return new()
                    {
                        id = friendRequest.id,
                        status = friendRequest.status,
                        created_at = friendRequest.createdAt,
                        from_user = UserInfoEntity.Response.User.FromPlayerProfile(
                            PlayerProfile.Load(friendRequest.fromUser))
                    };
                }
            }
        }
    }
}