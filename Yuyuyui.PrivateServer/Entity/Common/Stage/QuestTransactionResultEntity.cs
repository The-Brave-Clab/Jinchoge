using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer;

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

    protected override async Task ProcessRequest()
    {
        var playerId = await GetPlayerIdFromCookies();

        long stageId = long.Parse(GetPathParameter("stage_id"));
        long transactionId = long.Parse(GetPathParameter("transaction_id"));
            
        Request requestObj = Deserialize<Request>(requestBody)!;

        QuestTransaction transaction = await QuestTransaction.Load(transactionId);
            
        // Validate here?

        Stage dbStage;
        Episode dbEpisode;
        Chapter dbChapter;

        await using (QuestsContext questsDb = new())
        {
            dbStage = questsDb.Stages.First(s => s.Id == transaction.stageId);
            dbEpisode = questsDb.Episodes.First(e => e.Id == dbStage.EpisodeId);
            dbChapter = questsDb.Chapters.First(c => c.Id == dbEpisode.ChapterId);
        }


        PlayerProfile player;

        StageProgress stageProgress;
        EpisodeProgress episodeProgress;
        ChapterProgress chapterProgress;

        ChapterEntity.Response.Chapter chapter;
        EpisodeEntity.Response.Episode episode;
        StageEntity.Response.Stage stage;

        await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
        {
            player = await PlayerProfile.Load(playerId.code);

            stageProgress = await StageProgress.GetOrCreate(player, dbStage.Id);
            episodeProgress = await EpisodeProgress.GetOrCreate(player, dbEpisode.Id);
            chapterProgress = await ChapterProgress.GetOrCreate(player, dbChapter.Id);

            chapter = await ChapterEntity.Response.Chapter.GetFromDatabase(dbChapter, player);
            episode = await EpisodeEntity.Response.Episode.GetFromDatabase(dbEpisode, player);
            stage = await StageEntity.Response.Stage.GetFromDatabase(dbStage, player);
        }

        // The references are validated when updating the transaction.
        // We only update the progress data here.
        stageProgress.finished = stageProgress.finished || 
                                 requestObj.battle_result.finished_score_scenario;
        stageProgress.finishedInTime = stageProgress.finishedInTime || 
                                       requestObj.battle_result.finished_score_speed;
        stageProgress.finishedNoInjury = stageProgress.finishedNoInjury ||
                                         requestObj.battle_result.finished_score_no_injury;
        await stageProgress.Save();

        bool isScenario = dbStage.Kind == 0;

        // TODO: check episode & stage finished status here!

        Response responseObj = new()
        {
            chapter = chapter,
            episode = episode,
            stage = stage,
            battle_result = new(), // TODO
            title_items = null, // TODO
        };

        responseObj.chapter.id = chapterProgress.id;
        responseObj.episode.id = episodeProgress.id;
        responseObj.stage.id = stageProgress.id;

        if (!isScenario)
        {
            responseObj.title_items = new List<int>(); // TODO

            // fill in the battle result
            Deck deck = await Deck.Load(transaction.createdWith.using_deck_id!.Value); // This will not be null if battle
            responseObj.battle_result.deck = new()
            {
                id = deck.id, 
            };
                
            var deckUnits = await Unit.LoadMany(deck.units);
            foreach (var unit in deckUnits)
            {
                var rankInfo = requestObj.battle_result.deck_cards!.First(i => i.id == unit.id);

                // fill in the deck
                responseObj.battle_result.deck.cards.Add(
                    await Response.BattleResult.Deck.CardDataWithSupport.UpdateUnitAndGetData(unit, 
                        dbStage.CardExp * rankInfo.rank)); // TODO: rank calculation

                // fill in the character familiarities
                if (unit is { baseCardID: not null, supportCardID: not null })
                {
                    var baseCard = await Card.Load(unit.baseCardID!.Value);
                    var supportCard = await Card.Load(unit.supportCardID!.Value);
                    var baseMasterData = baseCard.MasterData();
                    var supportMasterData = supportCard.MasterData();

                    var familiarity = await player.GetCharacterFamiliarity(baseMasterData.CharacterId,
                        supportMasterData.CharacterId);
                    var gotFamiliarity = dbStage.Familiarity * rankInfo.rank; // TODO: familiarity calculation

                    var familiarityChange = familiarity.UpdateAndGetChange(gotFamiliarity);
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
        await player.Save();

        await transaction.Delete();
    }

    public class Request
    {
        public long quest_id { get; set; }
        public long transaction_id { get; set; }
        public BattleResult battle_result { get; set; } = new();
        public BattleLog? battle_log { get; set; } = null;
            
        public class BattleResult
        {
            public List<string> destroyed_wave_timeline_ids { get; set; } = [];
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

                    public async Task UpdateCardAndFillData(Card card, long gainedExp)
                    {
                        user_card_id = card.id;
                        master_id = card.master_id;
                        before_exp = card.exp;
                        before_level = card.level;
                        card.GainExp(gainedExp);
                        exp = card.exp;
                        level = card.level;
                            
                        await card.Save();
                    }

                    public static async Task<CardData> UpdateCardAndGetData(Card card, long gainedExp)
                    {
                        CardData result = new();
                        await result.UpdateCardAndFillData(card, gainedExp);
                        return result;
                    }
                }

                public class CardDataWithSupport : CardData
                {
                    public long id { get; set; } // unit id
                    public CardData support { get; set; } = new();
                    public CardData support_2 { get; set; } = new();

                    public static async Task<CardDataWithSupport> UpdateUnitAndGetData(Unit unit, long gainedExp)
                    {
                        CardDataWithSupport result = new() { id = unit.id };
                        var baseCard = await Card.Load(unit.baseCardID!.Value);
                        await result.UpdateCardAndFillData(baseCard, gainedExp);
                        if (unit.supportCardID != null)
                        {
                            var card = await Card.Load(unit.supportCardID!.Value);
                            result.support = await CardData.UpdateCardAndGetData(card, gainedExp);
                        }
                        if (unit.supportCard2ID != null)
                        {
                            var card = await Card.Load(unit.supportCard2ID!.Value);
                            result.support_2 = await CardData.UpdateCardAndGetData(card, gainedExp);
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