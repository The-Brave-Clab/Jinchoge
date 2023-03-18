using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class QuestTransactionResultEntity : BaseEntity<QuestTransactionResultEntity>
    {
        public QuestTransactionResultEntity(
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

            long stageId = long.Parse(GetPathParameter("stage_id"));
            long transactionId = long.Parse(GetPathParameter("transaction_id"));
            
            Request requestObj = Deserialize<Request>(requestBody)!;

            QuestTransaction transaction = QuestTransaction.Load(transactionId);
            
            // Validate here?
            
            using var questsDb = new QuestsContext();

            var dbStage = questsDb.Stages.First(s => s.Id == transaction.stageId);
            var dbEpisode = questsDb.Episodes.First(e => e.Id == dbStage.EpisodeId);
            var dbChapter = questsDb.Chapters.First(c => c.Id == dbEpisode.ChapterId);

            var stageProgress = StageProgress.GetOrCreate(player, dbStage.Id);
            var episodeProgress = EpisodeProgress.GetOrCreate(player, dbEpisode.Id);
            var chapterProgress = ChapterProgress.GetOrCreate(player, dbChapter.Id);
            
            // The references are validated when updating the transaction.
            // We only update the progress data here.
            stageProgress.finished = stageProgress.finished || 
                                     requestObj.battle_result.finished_score_scenario;
            stageProgress.finishedInTime = stageProgress.finishedInTime || 
                                           requestObj.battle_result.finished_score_speed;
            stageProgress.finishedNoInjury = stageProgress.finishedNoInjury ||
                                             requestObj.battle_result.finished_score_no_injury;
            stageProgress.Save();

            // TODO: check episode & stage finished status here!

            Response responseObj = new()
            {
                chapter = ChapterEntity.Response.Chapter.GetFromDatabase(dbChapter, player),
                episode = EpisodeEntity.Response.Episode.GetFromDatabase(dbEpisode, player),
                stage = StageEntity.Response.Stage.GetFromDatabase(dbStage, player),
                battle_result = new(), // TODO
                title_items = null, // TODO
            };

            responseObj.chapter.id = chapterProgress.id;
            responseObj.episode.id = episodeProgress.id;
            responseObj.stage.id = stageProgress.id;

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
            
            // Finished transaction, remove it
            player.transactions.questTransactions.Remove(transaction.stageId);
            player.Save();

            transaction.Delete();

            return Task.CompletedTask;
        }

        public class Request
        {
            public long quest_id { get; set; }
            public long transaction_id { get; set; }
            public BattleResult battle_result { get; set; } = new();
            public int? battle_log { get; set; } = null; // TODO
            
            public class BattleResult
            {
                public List<int> destroyed_wave_timeline_ids { get; set; } = new(); // TODO
                public bool finished_score_scenario { get; set; } // first star
                public bool finished_score_speed { get; set; } // second star
                public bool finished_score_no_injury { get; set; } // third star
                public long mvp_deck_card_id { get; set; } // 0 if not battle
                public int? deck_cards { get; set; } = null; // TODO
            }
        }

        public class Response
        {
            public ChapterEntity.Response.Chapter chapter { get; set; } = new();
            public EpisodeEntity.Response.Episode episode { get; set; } = new();
            public StageEntity.Response.Stage stage { get; set; } = new();
            public Dictionary<int, int> battle_result { get; set; } = new(); // TODO
            public int? title_items { get; set; } = null; // TODO
        }
    }
}