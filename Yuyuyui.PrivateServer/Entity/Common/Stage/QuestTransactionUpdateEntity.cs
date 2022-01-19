using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class QuestTransactionUpdateEntity : BaseEntity<QuestTransactionUpdateEntity>
    {
        public QuestTransactionUpdateEntity(
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

            long stageId = long.Parse(GetPathParameter("stage_id"));
            long transactionId = long.Parse(GetPathParameter("transaction_id"));
            
            // Ignored the request body since it's the same as the path parameter
            //Request request = Deserialize<Request>(requestBody)!;

            QuestTransaction transaction = QuestTransaction.Load(transactionId);
            
            // Validate here?
            
            using var questsDb = new QuestsContext();

            var dbStage = questsDb.Stages.First(s => s.Id == transaction.stageId);
            var dbEpisode = questsDb.Episodes.First(e => e.Id == dbStage.EpisodeId);
            var dbChapter = questsDb.Chapters.First(c => c.Id == dbEpisode.ChapterId);

            var stageProgress = StageProgress.GetOrCreate(player, dbStage.Id);
            var episodeProgress = EpisodeProgress.GetOrCreate(player, dbEpisode.Id);
            var chapterProgress = ChapterProgress.GetOrCreate(player, dbChapter.Id);

            if (!episodeProgress.stages.Contains(stageProgress.id))
            {
                episodeProgress.stages.Add(stageProgress.id);
                episodeProgress.Save();
            }

            if (!chapterProgress.episodes.Contains(episodeProgress.id))
            {
                chapterProgress.episodes.Add(episodeProgress.id);
                chapterProgress.Save();
            }

            Response responseObj = new()
            {
                boss = new(), // TODO
                chapter = Chapter.GetFromDatabase(dbChapter, player),
                episode = EpisodeEntity.Response.Episode.GetFromDatabase(dbEpisode, player),
                stage = StageEntity.Response.Stage.GetFromDatabase(dbStage, player),
                battle_info = new(), // TODO
                deck = new(), // TODO
                tree_hp = 3,
                brave_systems = new(), // TODO
                enemies = new(), // TODO
                wave_timelines = new(), // TODO
                game_mode_rule = new() // TODO: What is this?
                {
                    id = 1,
                    result_type = 1
                }
            };
            
            responseObj.chapter.id = chapterProgress.id;
            responseObj.episode.id = episodeProgress.id;
            responseObj.stage.id = stageProgress.id;

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        // public class Request
        // {
        //     public long quest_id { get; set; }
        //     public long transaction_id { get; set; }
        // }

        public class Response
        {
            public Dictionary<int, int> boss { get; set; } = new(); // TODO
            public Chapter chapter { get; set; } = new();
            public EpisodeEntity.Response.Episode episode { get; set; } = new();
            public StageEntity.Response.Stage stage { get; set; } = new();
            public Dictionary<int, int> battle_info { get; set; } = new(); // TODO
            public Dictionary<int, int> deck { get; set; } = new(); // TODO
            public int tree_hp { get; set; } = 3; // Is this fixed?
            public List<int> brave_systems { get; set; } = new(); // TODO
            public List<int> enemies { get; set; } = new(); // TODO
            public List<int> wave_timelines { get; set; } = new(); // TODO
            public GameModeRule game_mode_rule { get; set; } = new(); // TODO: What is this?

            public class GameModeRule
            {
                public long id { get; set; }
                public long result_type { get; set; }
            }
        }
    }
}