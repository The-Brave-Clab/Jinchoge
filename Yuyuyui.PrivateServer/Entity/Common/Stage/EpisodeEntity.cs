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
            Config config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();
            
            Utils.Log("Path parameters:");
            foreach (var pathParameter in pathParameters)
            {
                Utils.Log($"\t{pathParameter.Key} = {pathParameter.Value}");
            }

            long chapterId = long.Parse(GetPathParameter("chapter_id"));

            Utils.LogWarning("Finished status not filled!");

            using var questsDb = new QuestsContext();
            Response responseObj = new()
            {
                episodes = questsDb.Episodes
                    .Where(e => e.ChapterId == chapterId)
                    .Select(e => Response.Episode.GetFromDatabase(e, player))
                    .ToDictionary(e => e.id, e => e)
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IDictionary<long, Episode> episodes { get; set; } = new Dictionary<long, Episode>();

            public class Episode
            {
                public long id { get; set; } // When dealing with transaction, this should be the id of the player progress
                public long master_id { get; set; } // Don't know the difference
                public bool finish { get; set; }
                public string detail_url { get; set; } = "";

                public static Episode GetFromDatabase(Yuyuyui.PrivateServer.DataModel.Episode dbEpisode,
                    PlayerProfile player)
                {
                    return new()
                    {
                        id = dbEpisode.Id,
                        master_id = dbEpisode.Id,
                        finish = player.progress.episodes.ContainsKey(dbEpisode.Id) && 
                                 EpisodeProgress.Load(player.progress.episodes[dbEpisode.Id]).finished,
                        detail_url = $"https://article.yuyuyui.jp/article/episodes/{dbEpisode.Id}"
                    };
                }
            }
        }
    }
}