using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer.Entity.Events;

public class EventStageEntity : BaseEntity<EventStageEntity>
{
    public EventStageEntity(
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

        Utils.Log("Path parameters:");
        foreach (var pathParameter in pathParameters)
        {
            Utils.Log($"\t{pathParameter.Key} = {pathParameter.Value}");
        }

        long chapterId = long.Parse(GetPathParameter("specialChapterId"));
        long episodeId = long.Parse(GetPathParameter("specialEpisodeId"));

        using var eventStoriesDb = new EventStoriesContext();
        Response responseObj = new()
        {
            // Checking for chapter id might not be necessary
            stages = eventStoriesDb.SpecialStages.Where(s => s.SpecialChapterId == chapterId && s.SpecialEpisodeId == episodeId)
                .Select(s => Response.Stage.GetFromDatabase(s, player))
                .ToDictionary(s => s.id, s => s)
        };

        responseBody = Serialize(responseObj);
        SetBasicResponseHeaders();

        return Task.CompletedTask;

        return Task.CompletedTask;
    }

    public class Response
    {
        public IDictionary<long, Stage> stages { get; set; } = new Dictionary<long, Stage>();

        public class Stage
        {
            public long
                id { get; set; } // When dealing with transaction, this should be the id of the player progress

            public long master_id { get; set; } // Don't know the difference
            
            public long special_stage_id { get; set; } // Don't know the difference
            public bool finish { get; set; }
            public int score_finished_count { get; set; } // star count?
            public bool locked { get; set; }
            public float campaign_exchange_point_rate { get; set; } // ?
            public int campaign_stamina_rate { get; set; } // ?
            public long? end_at_stamina_campaign { get; set; } = null; // ? unixtime?
            public long stage_by_level_end_at { get; set; } // unixtime
            public bool play_auto_clear { get; set; } // flag for enabling auto play?
            public bool no_friend { get; set; } // variable type unknown, only saw null

            public static Stage GetFromDatabase(SpecialStage dbStage, PlayerProfile player)
            {
                Stage result = new()
                {
                    id = dbStage.Id,
                    master_id = dbStage.Id,
                    finish = false,
                    score_finished_count = 0,
                    locked = false, // TODO
                    campaign_exchange_point_rate = dbStage.ExchangePointRate,
                    campaign_stamina_rate = 1, // TODO
                    end_at_stamina_campaign = null, // ?
                    stage_by_level_end_at = 1869663600,
                    play_auto_clear = false,
                    no_friend = true
                };
                
                return result;
            }
        }
    }
}