using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer;

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

    protected override async Task ProcessRequest()
    {
        Response responseObj = await GetChapters();

        responseBody = Serialize(responseObj);
        SetBasicResponseHeaders();
    }

    protected virtual async Task<Response> GetChapters()
    {
        var playerId = await GetPlayerIdFromCookies();
            
        // Utils.LogWarning("Locked status not filled!");

        List<Chapter> chapters;
        await using (QuestsContext questsDb = new())
        {
            chapters = questsDb.Chapters.ToList();
        }


        PlayerProfile player;
        Response.Chapter[] responseChapters;
        await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
        {
            player = await PlayerProfile.Load(playerId.code);
            responseChapters = await chapters
                .Select(c => Response.Chapter.GetFromDatabase(c, player))
                .WhenAll();
        }

        Response response = new()
        {
            chapters = responseChapters.ToDictionary(c => c.id, c => c)
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

            public static async Task<Chapter> GetFromDatabase(Yuyuyui.PrivateServer.DataModel.Chapter dbChapter, PlayerProfile player)
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

                if (player.progress.chapters.TryGetValue(dbChapter.Id, out var chapter))
                {
                    result.new_released = false;
                    result.completed = (await ChapterProgress.Load(chapter)).finished;
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