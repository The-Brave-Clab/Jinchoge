using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class EpisodeEntity : BaseEntity<EpisodeEntity>
    {
        public EpisodeEntity(
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
            var player = await GetPlayerFromCookies();

            long chapterId = long.Parse(GetPathParameter("chapter_id"));

            // Utils.LogWarning("Finished status not filled!");

            List<Episode> episodes;
            await using (QuestsContext questsDb = new())
            {
                episodes = questsDb.Episodes
                    .Where(e => e.ChapterId == chapterId)
                    .ToList();
            }

            var responseEpisodes = await episodes
                .Select(e => Response.Episode.GetFromDatabase(e, player))
                .WhenAll();

            Response responseObj = new()
            {
                episodes = responseEpisodes.ToDictionary(e => e.id, e => e)
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }

        public class Response
        {
            public IDictionary<long, Episode> episodes { get; set; } = new Dictionary<long, Episode>();

            public class Episode
            {
                public long id { get; set; } // When dealing with transaction, this should be the id of the player progress
                public long master_id { get; set; }
                public bool finish { get; set; }
                public string detail_url { get; set; } = "";

                public static async Task<Episode> GetFromDatabase(Yuyuyui.PrivateServer.DataModel.Episode dbEpisode,
                    PlayerProfile player)
                {
                    return new()
                    {
                        id = dbEpisode.Id,
                        master_id = dbEpisode.Id,
                        finish = player.progress.episodes.ContainsKey(dbEpisode.Id) &&
                                 (await EpisodeProgress.Load(player.progress.episodes[dbEpisode.Id])).finished,
                        detail_url = $"https://article.yuyuyui.jp/article/episodes/{dbEpisode.Id}"
                    };
                }
            }
        }
    }
}