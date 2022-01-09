namespace Yuyuyui.PrivateServer
{
    public class UserInfoEntity : BaseEntity<UserInfoEntity>
    {
        public UserInfoEntity(
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
            var player = PlayerProfile.Load(GetPathParameter("user_id"));
            
            Response responseObj = new()
            {
                user = player
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public User user = new();
            
            public class User
            {
                public string id { get; set; } = "";
                public int level { get; set; }
                public string nickname { get; set; } = "";
                public string comment { get; set; } = "";
                public long accessed_at { get; set; }
                public int fellowship_count { get; set; }
                public long? title_item_id { get; set; } = null;
                public Unit.CardWithSupport leader_card { get; set; } = new();

                public static implicit operator User(PlayerProfile player)
                {
                    return new()
                    {
                        id = player.id.code,
                        level = player.data.level,
                        nickname = player.profile.nickname,
                        comment = player.profile.comment,
                        accessed_at = player.data.lastActive,
                        fellowship_count = player.friends.Count,
                        title_item_id = player.data.titleItemID,
                        leader_card = Unit.Load(Deck.Load(player.decks[0]).leaderUnitID)!
                    };
                }
            }
        }
    }
}