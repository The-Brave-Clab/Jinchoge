using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class ChapterEntity : BaseEntity<ChapterEntity>
    {
        public ChapterEntity(
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

            using var questsDb = new QuestsContext();
            Response responseObj = GetChapters(questsDb);

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        protected virtual Response GetChapters(QuestsContext questsDb)
        {
            var player = GetPlayerFromCookies();
            
            // Utils.LogWarning("Locked status not filled!");
                
            Response response = new()
            {
                chapters = questsDb.Chapters
                    .Select(c => Response.Chapter.GetFromDatabase(c, player))
                    .ToDictionary(c => c.id, c => c)
            };

            return response;
        }

        public class Response
        {
            public IDictionary<long, Chapter> chapters { get; set; } = new Dictionary<long, Chapter>();
            
            public class Chapter
            {
                public long id { get; set; } // When dealing with transaction, this should be the id of the player progress
                public long master_id { get; set; }
                public bool new_released { get; set; }
                public bool completed { get; set; }
                public int kind { get; set; }
                public long start_at { get; set; }
                public long end_at { get; set; }
                public string detail_url { get; set; } = "";
                public long stack_point { get; set; }
                public bool locked { get; set; }
                public int available_user_level { get; set; }

                public static Chapter GetFromDatabase(Yuyuyui.PrivateServer.DataModel.Chapter dbChapter, PlayerProfile player)
                {
                    Chapter result = new Chapter
                    {
                        id = dbChapter.Id,
                        master_id = dbChapter.Id,
                        kind = 0,
                        start_at = 0,
                        end_at = 0,
                        detail_url = $"https://article.yuyuyui.jp/article/episodes/{dbChapter.Id}",
                        stack_point = 0,
                        locked = false, // TODO
                        new_released = false,
                        completed = false,
                        available_user_level = 0 // TODO
                    };

                    if (player.progress.chapters.ContainsKey(dbChapter.Id))
                    {
                        result.new_released = false;
                        result.completed = ChapterProgress.Load(player.progress.chapters[dbChapter.Id]).finished;
                    }
                    else
                    {
                        result.new_released = true;
                    }

                    return result;
                }
            }
        }
    }
}