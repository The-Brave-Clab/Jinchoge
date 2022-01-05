namespace Yuyuyui.PrivateServer
{
    public class BadgeEntity : BaseEntity<BadgeEntity>
    {
        public BadgeEntity(
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

            Utils.LogWarning("Stub API!");

            Response responseObj = new()
            {
                badge = new()
                {
                    has_complete_mission = true,
                    has_complete_daily_mission = true,
                    has_present = true,
                    has_fellow_request = true,
                    has_complete_club_working = true,
                    end_at_exchange = Utils.CurrentUnixTime() + 120,
                    has_exchangeable_bingo = true,
                    end_at_event = Utils.CurrentUnixTime() + 180,
                    end_at_playback_event = Utils.CurrentUnixTime() + 240,
                    new_title = 1,
                    new_album_categories = new Dictionary<string, IList<int>>
                    {
                        {
                            "1", new List<int>
                            {
                                1,
                            }
                        },
                    },
                    end_at_collab_event = Utils.CurrentUnixTime() + 320
                }
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public BadgeData badge { get; set; } = new();

            public class BadgeData
            {
                public bool has_complete_mission { get; set; }
                public bool has_complete_daily_mission { get; set; }
                public bool has_present { get; set; }
                public bool has_fellow_request { get; set; }
                public bool has_complete_club_working { get; set; }
                public long? end_at_exchange { get; set; } = null; // unixtime
                public bool has_exchangeable_bingo { get; set; }
                public long? end_at_event { get; set; } = null; // assumption
                public long? end_at_playback_event { get; set; } = null; // assumption
                public int new_title { get; set; } // unknown

                public IDictionary<string, IList<int>> new_album_categories { get; set; } =
                    new Dictionary<string, IList<int>>(); // assumption

                public long? end_at_collab_event { get; set; } = null; // assumption
            }
        }
    }
}