using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class StageEntity : BaseEntity<StageEntity>
    {
        public StageEntity(
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
            long episodeId = long.Parse(GetPathParameter("episode_id"));

            Utils.LogWarning("Many status not filled.");

            Response responseObj = new()
            {
                // Checking for chapter id might not be necessary
                stages = DatabaseContexts.Quests.Stages.Where(s => s.ChapterId == chapterId && s.EpisodeId == episodeId)
                    .Select(s => Response.Stage.GetFromDatabase(s, player))
                    .ToDictionary(s => s.id, s => s)
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

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
                public bool finish { get; set; }
                public int score_finished_count { get; set; } // star count?
                public bool locked { get; set; }
                public float campaign_exchange_point_rate { get; set; } // ?
                public int campaign_stamina_rate { get; set; } // ?
                public long? end_at_stamina_campaign { get; set; } = null; // ? unixtime?
                public long stage_by_level_end_at { get; set; } // unixtime
                public bool play_auto_clear { get; set; } // flag for enabling auto play?
                public int? no_friend { get; set; } = null; // variable type unknown, only saw null

                public static Stage GetFromDatabase(Yuyuyui.PrivateServer.DataModel.Stage dbStage, PlayerProfile player)
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
                        no_friend = dbStage.NoFriend
                    };

                    if (player.progress.stages.ContainsKey(dbStage.Id))
                    {
                        StageProgress progress = StageProgress.Load(player.progress.stages[dbStage.Id]);
                        result.finish = progress.finished;

                        // Scenario only stages will also affect the total star count of the episode
                        // so we manually set it to zero
                        if (dbStage.Kind == 0)
                        {
                            result.score_finished_count = 0;
                        }
                        else if (progress.finished)
                        {
                            result.score_finished_count = 1;
                            if (progress.finishedInTime)
                                ++result.score_finished_count;
                            if (progress.finishedNoInjury)
                                ++result.score_finished_count;
                        }
                        else
                        {
                            result.score_finished_count = 0;
                        }
                    }

                    // TODO: check config of unlocking all difficulties, left it here for easier debugging
                    // Leave the scenario only stage as-is, 
                    // for better indication of whether the player has already watched it.
                    if (dbStage.Kind != 0)
                    {
                        // To trick the client to unlock the hard and expert difficulty for us,
                        // we can just 3 star every non-scenario stage.
                        result.finish = true;
                        result.score_finished_count = 3;
                    }

                    return result;
                }
            }
        }
    }
}