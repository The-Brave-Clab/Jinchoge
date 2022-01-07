using System.Net;

namespace Yuyuyui.PrivateServer
{
    public class ChapterEntity : BaseEntity<ChapterEntity>
    {
        public ChapterEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config,
            IPEndPoint localEndPoint)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config, localEndPoint)
        {
        }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();

            Response responseObj = GetChapters();

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        protected virtual Response GetChapters()
        {
            Utils.LogWarning("Stub API! This might come from master_data?");
            
            return new()
            {
                chapters = new Dictionary<int, Chapter>
                {
                    {
                        1, new()
                        {
                            id = 1,
                            master_id = 1,
                            kind = 0,
                            start_at = 0,
                            end_at = 0,
                            detail_url = "https://article.yuyuyui.jp/article/episodes/1",
                            stack_point = 0,
                            locked = false,
                            new_released = false,
                            completed = false,
                            available_user_level = 0
                        }
                    },
                    {
                        2, new()
                        {
                            id = 2,
                            master_id = 2,
                            kind = 0,
                            start_at = 0,
                            end_at = 0,
                            detail_url = "https://article.yuyuyui.jp/article/episodes/2",
                            stack_point = 0,
                            locked = false,
                            new_released = true,
                            completed = false,
                            available_user_level = 0
                        }
                    },
                    {
                        3, new()
                        {
                            id = 3,
                            master_id = 3,
                            kind = 0,
                            start_at = 0,
                            end_at = 0,
                            detail_url = "https://article.yuyuyui.jp/article/episodes/3",
                            stack_point = 0,
                            locked = false,
                            new_released = true,
                            completed = false,
                            available_user_level = 0
                        }
                    },
                    {
                        4, new()
                        {
                            id = 4,
                            master_id = 4,
                            kind = 0,
                            start_at = 0,
                            end_at = 0,
                            detail_url = "https://article.yuyuyui.jp/article/episodes/4",
                            stack_point = 0,
                            locked = false,
                            new_released = true,
                            completed = false,
                            available_user_level = 0
                        }
                    },
                    {
                        5, new()
                        {
                            id = 5,
                            master_id = 5,
                            kind = 0,
                            start_at = 0,
                            end_at = 0,
                            detail_url = "https://article.yuyuyui.jp/article/episodes/5",
                            stack_point = 0,
                            locked = false,
                            new_released = true,
                            completed = false,
                            available_user_level = 0
                        }
                    },
                    {
                        6, new()
                        {
                            id = 6,
                            master_id = 6,
                            kind = 0,
                            start_at = 0,
                            end_at = 0,
                            detail_url = "https://article.yuyuyui.jp/article/episodes/6",
                            stack_point = 0,
                            locked = false,
                            new_released = true,
                            completed = false,
                            available_user_level = 0
                        }
                    },
                    {
                        7, new()
                        {
                            id = 7,
                            master_id = 7,
                            kind = 0,
                            start_at = 0,
                            end_at = 0,
                            detail_url = "https://article.yuyuyui.jp/article/episodes/7",
                            stack_point = 0,
                            locked = false,
                            new_released = true,
                            completed = false,
                            available_user_level = 0
                        }
                    },
                    {
                        8, new()
                        {
                            id = 8,
                            master_id = 8,
                            kind = 0,
                            start_at = 0,
                            end_at = 0,
                            detail_url = "https://article.yuyuyui.jp/article/episodes/8",
                            stack_point = 0,
                            locked = false,
                            new_released = true,
                            completed = false,
                            available_user_level = 0
                        }
                    },
                    {
                        9, new()
                        {
                            id = 9,
                            master_id = 9,
                            kind = 0,
                            start_at = 0,
                            end_at = 0,
                            detail_url = "https://article.yuyuyui.jp/article/episodes/9",
                            stack_point = 0,
                            locked = false,
                            new_released = true,
                            completed = false,
                            available_user_level = 0
                        }
                    }
                }
            };
        }

        public class Response
        {
            public IDictionary<int, Chapter> chapters { get; set; } = new Dictionary<int, Chapter>();
        }
    }
}