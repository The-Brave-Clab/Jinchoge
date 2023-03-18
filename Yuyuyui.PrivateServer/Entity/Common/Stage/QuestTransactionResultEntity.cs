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

            bool isScenario = dbStage.Kind == 0;

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

            if (!isScenario)
            {
                responseObj.title_items = new List<int>(); // TODO

                using var cardsDb = new CardsContext();

                // fill in the battle result
                Deck deck = Deck.Load(transaction.createdWith.using_deck_id!.Value); // This will not be null if battle
                responseObj.battle_result.deck = new()
                {
                    id = deck.id, 
                };
                foreach (var unitId in deck.units)
                {
                    var unit = Unit.Load(unitId);
                    var rankInfo = requestObj.battle_result.deck_cards!.First(i => i.id == unit.id);

                    // fill in the deck
                    responseObj.battle_result.deck.cards.Add(
                        Response.BattleResult.Deck.CardDataWithSupport.UpdateUnitAndGetData(cardsDb, unit, 
                            dbStage.CardExp * rankInfo.rank)); // TODO: rank calculation

                    // fill in the character familiarities
                    using var charactersDb = new CharactersContext();
                    if (unit.baseCardID != null && unit.supportCardID != null)
                    {
                        var baseCard = Card.Load(unit.baseCardID!.Value);
                        var supportCard = Card.Load(unit.supportCardID!.Value);
                        var baseMasterData = baseCard.MasterData(cardsDb);
                        var supportMasterData = supportCard.MasterData(cardsDb);

                        var familiarity =
                            player.GetCharacterFamiliarity(baseMasterData.CharacterId, supportMasterData.CharacterId);
                        var gotFamiliarity = dbStage.Familiarity * rankInfo.rank; // TODO: familiarity calculation

                        var familiarityChange = familiarity.UpdateAndGetChange(charactersDb, gotFamiliarity);
                        responseObj.battle_result.familiarities.Add(familiarityChange);
                    }
                    
                    // fill in the user data
                    // TODO: user data not processed yet
                    responseObj.battle_result.user = new()
                    {
                        exp = player.data.exp,
                        level = player.data.level,
                        money = player.data.money,
                        friend_point = player.data.friendPoint,
                        t_point = player.data.exchangePoint,
                        before_exp = player.data.exp,
                        before_level = player.data.level,
                        before_money = player.data.money,
                        before_friend_point = player.data.friendPoint,
                        before_t_point = player.data.exchangePoint,
                    };
                }

                // drop rewards
                // TODO
                responseObj.battle_result.drop_rewards = new List<Response.BattleResult.DropReward>();
                responseObj.battle_result.base_drop_rewards = new List<Response.BattleResult.DropReward>();
                responseObj.battle_result.score_rewards = new List<Response.BattleResult.DropReward>();

                // supporter
                // TODO
                responseObj.battle_result.supporters = new List<Response.BattleResult.SupporterData>();

                responseObj.battle_result.got_free_rare_gacha_right = false;
            }

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
            public BattleLog? battle_log { get; set; } = null;
            
            public class BattleResult
            {
                public List<string> destroyed_wave_timeline_ids { get; set; } = new();
                public bool finished_score_scenario { get; set; } // first star
                public bool finished_score_speed { get; set; } // second star
                public bool finished_score_no_injury { get; set; } // third star
                public long mvp_deck_card_id { get; set; } // 0 if not battle
                public IList<DeckCardInfo>? deck_cards { get; set; } = null;

                public class DeckCardInfo
                {
                    public long id { get; set; } // deck id
                    public int rank { get; set; }
                }
            }

            public class BattleLog
            {
                public float battleTime { get; set; }
                public float battleSpeed { get; set; }
                public IList<PairLog> pairLog { get; set; } = new List<PairLog>();

                public class PairLog
                {
                    public IList<CardLog> cardsLog { get; set; } = new List<CardLog>();
                    public int rankUpCount { get; set; }
                    public long skillId { get; set; }
                    public int skillLv { get; set; }
                    public int skillCount { get; set; }
                    public int maxSkillDamage { get; set; }
                }

                public class CardLog
                {
                    public long cardId { get; set; }
                    public long cardMasterId { get; set; }
                    public int maxDamage { get; set; }
                    public int maxTakeDamage { get; set; }
                    public int attack { get; set; }
                    public float cri { get; set; }
                    public float speed { get; set; }
                }
            }
        }

        public class Response
        {
            public ChapterEntity.Response.Chapter chapter { get; set; } = new();
            public EpisodeEntity.Response.Episode episode { get; set; } = new();
            public StageEntity.Response.Stage stage { get; set; } = new();
            public BattleResult battle_result { get; set; } = new();
            public IList<int>? title_items { get; set; } = null; // TODO

            public class BattleResult
            {
                public Deck deck { get; set; } = new();
                public IList<CharacterFamiliarityChange> familiarities { get; set; } = new List<CharacterFamiliarityChange>();
                public User user { get; set; } = new();
                public IList<DropReward> drop_rewards { get; set; } = new List<DropReward>();
                public IList<DropReward> base_drop_rewards { get; set; } = new List<DropReward>();
                public IList<DropReward> score_rewards { get; set; } = new List<DropReward>();
                public IList<SupporterData> supporters { get; set; } = new List<SupporterData>();
                public bool got_free_rare_gacha_right { get; set; } // ?

                public class Deck
                {
                    public long id { get; set; } // deck id
                    public IList<CardDataWithSupport> cards { get; set; } = new List<CardDataWithSupport>(); // without friend

                    public class CardData
                    {
                        public long user_card_id { get; set; }
                        public long master_id { get; set; }
                        public long exp { get; set; }
                        public int level { get; set; }
                        public long before_exp { get; set; }
                        public int before_level { get; set; }

                        public void UpdateCardAndFillData(CardsContext cardsDb, Card card, long gainedExp)
                        {
                            user_card_id = card.id;
                            master_id = card.master_id;
                            before_exp = card.exp;
                            before_level = card.level;
                            card.GainExp(cardsDb, gainedExp);
                            exp = card.exp;
                            level = card.level;
                            
                            card.Save();
                        }

                        public static CardData UpdateCardAndGetData(CardsContext cardsDb, Card card, long gainedExp)
                        {
                            CardData result = new();
                            result.UpdateCardAndFillData(cardsDb, card, gainedExp);
                            return result;
                        }
                    }

                    public class CardDataWithSupport : CardData
                    {
                        public long id { get; set; } // unit id
                        public object support { get; set; } = new(); // CardData
                        public object support_2 { get; set; } = new(); // CardData

                        public static CardDataWithSupport UpdateUnitAndGetData(CardsContext cardsDb, Unit unit, long gainedExp)
                        {
                            CardDataWithSupport result = new() { id = unit.id };
                            var baseCard = Card.Load(unit.baseCardID!.Value);
                            result.UpdateCardAndFillData(cardsDb, baseCard, gainedExp);
                            if (unit.supportCardID != null)
                            {
                                var card = Card.Load(unit.supportCardID!.Value);
                                result.support = CardData.UpdateCardAndGetData(cardsDb, card, gainedExp);
                            }
                            if (unit.supportCard2ID != null)
                            {
                                var card = Card.Load(unit.supportCard2ID!.Value);
                                result.support_2 = CardData.UpdateCardAndGetData(cardsDb, card, gainedExp);
                            }

                            return result;
                        }
                    }
                }

                public class User
                {
                    public long exp { get; set; }
                    public int level { get; set; }
                    public long money { get; set; }
                    public int friend_point { get; set; }
                    public long before_exp { get; set; }
                    public int before_level { get; set; }
                    public long before_money { get; set; }
                    public int before_friend_point { get; set; }
                    public int t_point { get; set; } = -1;
                    public int before_t_point { get; set; } = -1;
                }

                public class DropReward
                {
                    public ItemCategory item_category;
                    public long item_master_id;
                    public DropBoxType drop_box_type;
                    public bool discarded;
                    public int quantity;
                }

                public class SupporterData
                {
                    public string user_id { get; set; } = "";
                    public bool fellow { get; set; }
                    public bool requestable { get; set; }
                    public int level { get; set; }
                    public string nickname { get; set; } = "";
                    public long? title_item_id { get; set; } = null;
                    public GuestEntity.Response.SupporterData.CardDataWithSupport leader_card { get; set; } = new();
                }
            }
        }
    }
}