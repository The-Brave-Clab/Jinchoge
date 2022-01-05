namespace Yuyuyui.PrivateServer
{
    public class BannerEntity : BaseEntity<BannerEntity>
    {
        public BannerEntity(
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
            Utils.LogWarning("Stub API!");
            
            Response responseObj = new()
            {
                banners = new List<Banner>
                {
                    new()
                    {
                        image_id = 37930,
                        transition_screen_kind = "gacha/limited",
                        transition_url = "",
                        available_user_level = 0
                    },
                    new()
                    {
                        image_id = 1529,
                        transition_screen_kind = "topics",
                        transition_url = "4522",
                        available_user_level = 0
                    },
                    new()
                    {
                        image_id = 1441,
                        transition_screen_kind = "story",
                        transition_url = "",
                        available_user_level = 0
                    },
                    new()
                    {
                        image_id = 71970,
                        transition_screen_kind = "shop/package",
                        transition_url = "",
                        available_user_level = 0
                    },
                    new()
                    {
                        image_id = 90020,
                        transition_screen_kind = "",
                        transition_url = "https://yuyuyu.tv/churutto/",
                        available_user_level = 0
                    },
                }
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IList<Banner> banners { get; set; } = new List<Banner>();
        }
    }
}