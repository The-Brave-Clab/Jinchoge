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

            Utils.LogError("Very unfinished API, please check");


            List<long> stageIDs = new List<long>
            {
                10010101,
                10010102,
                10010103,
                10010104,
                10010105,
                10010106,
                10010107,
                10010108
            };
            
            Response responseObj = new()
            {
                stages = stageIDs.Select(id => new Response.Stage()
                {
                    id = id,
                    master_id = id,
                    finish = false,
                    score_finished_count = 0,
                    locked = false,
                    campaign_exchange_point_rate = 1.0f,
                    campaign_stamina_rate = 1,
                    end_at_stamina_campaign = null,
                    stage_by_level_end_at = 1869663600,
                    play_auto_clear = false,
                    no_friend = null
                }).ToDictionary(s => s.id, s => s)
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
                public long id { get; set; }
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
            }
        }
    }
}