using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
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
            RouteConfig config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();

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

            Response responseObj = new();

            if (dbStage.Kind != 0) // not scenario, means battle stage
            {
                var stageData = ProxyUtils.ReadAllTextFromAssemblyResources($"data.stages.{stageId}.yaml");
                var deserializer = new DeserializerBuilder()
                    .IgnoreUnmatchedProperties()
                    .Build();
                responseObj = deserializer.Deserialize<Response>(stageData);
                
                using var cardsDb = new CardsContext();
                using var accessoriesDb = new AccessoriesContext();
                using var charactersDb = new CharactersContext();
                // TODO: fill in the guest
                responseObj.deck = Response.BattleDeck.FromTransaction(cardsDb, accessoriesDb, charactersDb, transaction, player, null);
            }

            responseObj.chapter = ChapterEntity.Response.Chapter.GetFromDatabase(dbChapter, player);
            responseObj.episode = EpisodeEntity.Response.Episode.GetFromDatabase(dbEpisode, player);
            responseObj.stage = StageEntity.Response.Stage.GetFromDatabase(dbStage, player);
            
            responseObj.chapter.id = chapterProgress.id;
            responseObj.episode.id = episodeProgress.id;
            responseObj.stage.id = stageProgress.id;
            
            // TODO: if the stage costs stamina, do it here

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
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
            public Boss? boss { get; set; } = null;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
            public List<Boss>? multiple_bosses { get; set; } = null;
            public ChapterEntity.Response.Chapter chapter { get; set; } = new();
            public EpisodeEntity.Response.Episode episode { get; set; } = new();
            public StageEntity.Response.Stage stage { get; set; } = new();
            public BattleInfo battle_info { get; set; } = new();
            public BattleDeck deck { get; set; } = new();
            public int tree_hp { get; set; } = 3; // Is this fixed?
            public List<int> brave_systems { get; set; } = new(); // only saw empty for story stages
            public List<Enemy> enemies { get; set; } = new();
            public List<WaveTimeline> wave_timelines { get; set; } = new();
            public GameModeRule game_mode_rule { get; set; } = new();

            public class Boss
            {
                public string id { get; set; } = "";
                public long? drop_box_type { get; set; } = null;
                public int line_num { get; set; }
                public float pop_value { get; set; }
                public int pop_type { get; set; }
                public int pop_cond_type { get; set; }
                public int pop_cond_value { get; set; }
                public string enemy_id { get; set; } = "";
                public List<Status> statuses { get; set; } = new();

                public class Status
                {
                    public AttributeType element;
                    public int battle_level;
                    public float atk_level;
                    public float hp_level;
                    public Enemy enemy { get; set; } = new();
                    public AttributeResistances attr_resistances { get; set; } = new();
                }
            }

            public class BattleInfo
            {
                public int score_speed_expedition_second { get; set; }
                public int loop_start_time { get; set; }
                public int enemy_limit { get; set; }
            }

            public class Enemy
            {
                public string id { get; set; } = "";
                public long enemy_type { get; set; }
                public float image_scale { get; set; }
                public float radius { get; set; }
                public int attack { get; set; }
                public long hp { get; set; }
                public float attack_radius { get; set; }
                public float attack_pace { get; set; }
                public float move_speed { get; set; }
                public int defense { get; set; }
                public float avoid_rate { get; set; }
                public float hit_rate { get; set; }
                public int critical_point { get; set; }
                public long footing_point { get; set; }
                public int exp { get; set; }
                public int exp_per_attack { get; set; }
                public SizeType size_type { get; set; } // only saw SMALL
                public MoveType move_type { get; set; }
                public float move_to_x { get; set; }
                public float wait_time { get; set; }
                public float move_time { get; set; }
                public LineChangeMoveType lc_move_type { get; set; }
                public LineChangeTriggerType lc_trigger_type { get; set; }
                public int lc_max_count { get; set; }
                public float lc_interval { get; set; }
                public int battle_item_id { get; set; }
                public float battle_item_drop_rate { get; set; }
                public bool notice { get; set; }
                public string name { get; set; } = "";
                public bool vertex { get; set; }
                public int attack_effect_size { get; set; }
                public HpGaugeType hp_gauge_type { get; set; }
                public float as_first_interval { get; set; } // only saw 0
                public float hit_effect_height { get; set; }
                public List<ActiveSkill> active_skills { get; set; } = new();
                public List<SkillInfo> passive_skills { get; set; } = new();
                public int? character_type { get; set; } = null;
                public long? master_id { get; set; } = null;
                public SizeType? summon_size_type { get; set; } = null;

                public enum SizeType
                {
                    SMALL,
                    MEDIUM,
                    LARGE
                }

                public enum MoveType
                {
                    NORMAL,
                    ACTIVE_MOVE_STOP,
                    WAIT_AND_MOVE,
                    HATE,
                    HATE_ACTIVE_MOVE_STOP,
                    HATE_WAIT_AND_MOVE,
                    HATE_ACTIVESTOP_AND_WAITMOVE
                }

                public enum LineChangeMoveType
                {
                    NONE,
                    BLANK,
                    LOW_HP,
                    RANDOM
                }

                public enum LineChangeTriggerType
                {
                    NONE,
                    TIME,
                    ATTACK
                }

                public enum HpGaugeType
                {
                    NONE,
                    SHORT,
                    NOMAL,
                    BOSS
                }
            }

            public class WaveTimeline
            {
                public string id { get; set; } = "";
                public float pop_time { get; set; }
                public int battle_level { get; set; }
                public int line_num { get; set; }
                public long? drop_box_type { get; set; } = null;
                public string enemy_id { get; set; } = "";
                public float atk_level { get; set; }
                public float hp_level { get; set; }
                public AttributeType element { get; set; }
                public AttributeResistances attr_resistances { get; set; } = new();
            }

            public class GameModeRule
            {
                public long id { get; set; } = 1;
                public long result_type { get; set; } = 1;
            }

            public class AttributeResistances // Only saw all zero data
            {
                public int element_normal { get; set; } = 0;
                public int element_blue { get; set; } = 0;
                public int element_green { get; set; } = 0;
                public int element_red { get; set; } = 0;
                public int element_yellow { get; set; } = 0;
                public int element_purple { get; set; } = 0;
                public int type_short { get; set; } = 0;
                public int type_middle { get; set; } = 0;
                public int type_long { get; set; } = 0;
            }
            
            public class BattleDeck
            {
                public LeaderSkillInfo friend_leader_skill { get; set; } = new();
                public LeaderSkillInfo leader_skill { get; set; } = new();
                public IList<SkillInfo> stage_leader_skills { get; set; } = new List<SkillInfo>();
                public IList<BattleCardData> cards { get; set; } = new List<BattleCardData>();

                public static BattleDeck FromTransaction(CardsContext cardsDb, AccessoriesContext accessoriesDb,
                    CharactersContext charactersDb, QuestTransaction transaction, PlayerProfile player, PlayerProfile? guest)
                {
                    var deck = Deck.Load(transaction.createdWith.using_deck_id!.Value);

                    var leaderUnit = Unit.Load(deck.leaderUnitID);
                    var friendUnit = transaction.createdWith.supporting_deck_card_id == null
                        ? null
                        : Unit.Load(transaction.createdWith.supporting_deck_card_id.Value);

                    var result = new BattleDeck
                    {
                        leader_skill = LeaderSkillInfo.FromUnit(cardsDb, leaderUnit),
                        friend_leader_skill = LeaderSkillInfo.FromUnit(cardsDb, friendUnit),
                        stage_leader_skills = new List<SkillInfo>(), // This seems to be always empty?
                    };

                    int order = 1;
                    foreach (var unitId in deck.units)
                    {
                        var unit = Unit.Load(unitId);
                        if (unit.baseCardID == null) continue;
                        
                        result.cards.Add(BattleCardData.GetFromUnit(cardsDb, accessoriesDb, charactersDb, player, unit, order, unitId == deck.leaderUnitID, FriendType.OWNER));
                        ++order;
                    }

                    // TODO: For now we don't do friend units. Make sure to fill this when we do.
                    if (friendUnit != null)
                    {
                        
                    }

                    return result;
                }
            }

            public class BattleCardData : SubCardData
            {
                public long id { get; set; }
                public object supporter { get; set; } = new();
                public object supporter_2 { get; set; } = new();
                public object assist { get; set; } = new();
                public int order { get; set; }
                public SkillInfo active_skill { get; set; } = new();
                public List<Accessory> accessories { get; set; } = new();
                public int hp { get; set; }
                public bool leader { get; set; }
                public FriendType friend_type { get; set; }

                public static BattleCardData GetFromUnit(CardsContext cardsDb, AccessoriesContext accessoriesDb, CharactersContext charactersDb,
                    PlayerProfile player, Unit unit, int order, bool leader, FriendType friendType)
                {
                    Card baseCard = Card.Load(unit.baseCardID!.Value);
                    var masterCard = baseCard.MasterData(cardsDb);
                    Card? support = unit.supportCardID == null ? null : Card.Load(unit.supportCardID.Value);
                    Card? support2 = unit.supportCard2ID == null ? null : Card.Load(unit.supportCard2ID.Value);
                    Card? assist = unit.assistCardID == null ? null : Card.Load(unit.assistCardID.Value);

                    var result = new BattleCardData
                    {
                        id = unit.id,
                        base_info = CardBaseInfo.GetFromCard(cardsDb, baseCard),
                        supporter = GetFromCard(cardsDb, support),
                        supporter_2 = GetFromCard(cardsDb, support2),
                        assist = GetFromCard(cardsDb, assist),
                        order = order,
                        active_skill = new() { id = masterCard.ActiveSkillId!.Value, level = baseCard.active_skill_level },
                        accessories = unit.accessories
                            .Select(id => Accessory.GetFromUserAccessoryId(accessoriesDb, id))
                            .ToList(),
                        hp = unit.GetHP(cardsDb, charactersDb, player),
                        leader = leader,
                        friend_type = friendType
                    };
                    
                    result.FillFromCard(cardsDb, baseCard);

                    return result;
                }
            }

            public class CardBaseInfo
            {
                public AttributeType element { get; set; }
                public long character_type { get; set; }
                public long character_voice_type { get; set; }
                public float radius { get; set; }
                public int attack { get; set; }
                public int hp { get; set; }
                public float attack_radius { get; set; }
                public float attack_pace { get; set; }
                public float move_speed { get; set; }
                public int defense { get; set; }
                public float avoid_rate { get; set; }
                public float hit_rate { get; set; }
                public float critical_point { get; set; }
                public float footing_point { get; set; }
                public bool different_line_attack { get; set; }
                public long master_id { get; set; }
                public AttackType attack_type { get; set; }

                public static CardBaseInfo GetFromCard(CardsContext cardDb, Card card)
                {
                    var masterCard = card.MasterData(cardDb);

                    float growthValue = GrowthKind.GetValue(masterCard.GrowthKind);

                    return new()
                    {
                        element = (AttributeType)masterCard.Element,
                        character_type = masterCard.CharacterId,
                        character_voice_type = masterCard.CharacterId,
                        attack_radius = (AttackType)masterCard.AttackType == AttackType.LONG ? 10000 : 1,
                        defense = 1,
                        radius = 0.6f,
                        attack_type = (AttackType)masterCard.AttackType,
                        master_id = card.master_id,
                        avoid_rate = 0,
                        different_line_attack = (AttackType)masterCard.AttackType == AttackType.MIDDLE,
                        hit_rate = 1.0f,
                        attack_pace = masterCard.AttackPace,

                        hp = CalcUtil.CalcHitPointByLevel(
                            card.level, masterCard.MinLevel, masterCard.MaxLevel,
                            masterCard.MinHitPoint, masterCard.MaxHitPoint, growthValue,
                            card.potential, masterCard.LevelMaxHitPointBonus, masterCard.PotentialHitPointArgument),
                        attack = CalcUtil.CalcAttackByLevel(
                            card.level, masterCard.MinLevel, masterCard.MaxLevel,
                            masterCard.MinAttack, masterCard.MaxAttack, growthValue,
                            card.potential, masterCard.LevelMaxAttackBonus, masterCard.PotentialAttackArgument) / 8,
                        move_speed = CalcUtil.CalcParamByLevel(
                            card.level, masterCard.MinLevel, masterCard.MaxLevel,
                            masterCard.MinAgility, masterCard.MaxAgility, growthValue) / 40.0f,
                        critical_point = CalcUtil.CalcParamByLevel(
                            card.level, masterCard.MinLevel, masterCard.MaxLevel,
                            masterCard.MinCritical, masterCard.MaxCritical, growthValue),
                        footing_point = CalcUtil.CalcParamByLevel(
                            card.level, masterCard.MinLevel, masterCard.MaxLevel,
                            masterCard.MinWeight, masterCard.MaxWeight, growthValue),
                    };
                }
            }

            public class SubCardData
            {
                public CardBaseInfo base_info { get; set; } = new();
                public List<SkillInfo> passive_skills { get; set; } = new();

                public void FillFromCard(CardsContext cardDb, Card card)
                {
                    var masterCard = card.MasterData(cardDb);

                    base_info = CardBaseInfo.GetFromCard(cardDb, card);
                    passive_skills = new List<long?> { masterCard.SupportSkill1Id, masterCard.SupportSkill2Id }
                        .Where(id => id != null)
                        .Select(id => id!.Value)
                        .Select(id => new SkillInfo { id = id, level = card.support_skill_level })
                        .ToList();
                }

                public static object GetFromCard(CardsContext cardDb, Card? card)
                {
                    if (card == null) return new();

                    var result = new SubCardData();
                    result.FillFromCard(cardDb, card);
                    return result;
                }
            }
            
            public class Accessory
            {
                public long master_id { get; set; }
                public SkillInfo passive_skill { get; set; } = new();

                public static Accessory GetFromUserAccessoryId(AccessoriesContext accessoriesDb, long id)
                {
                    var accessory = Yuyuyui.PrivateServer.Accessory.Load(id);
                    var masterData = accessory.MasterData(accessoriesDb);

                    return new()
                    {
                        master_id = accessory.master_id,
                        passive_skill = new()
                        {
                            id = masterData.SkillId,
                            level = accessory.level
                        }
                    };
                }
            }

            public class SkillInfo
            {
                public long id { get; set; }
                public int level { get; set; }
            }

            public class LeaderSkillInfo
            {
                [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
                public long? id { get; set; } = null;

                public static LeaderSkillInfo FromUnit(CardsContext cardDb, Unit? unit)
                {
                    if (unit == null) return new();
                    var baseCard = unit.GetCard();
                    var masterCard = baseCard!.MasterData(cardDb);

                    return new()
                    {
                        id = masterCard.LeaderSkillId
                    };
                }
            }
            
            public enum AttributeType
            {
                NONE,
                RED,
                BLUE,
                GREEN,
                YELLOW,
                PURPLE
            }

            public enum AttackType
            {
                NONE,
                SHORT,
                MIDDLE,
                LONG,
                ASSIST
            }
            
            public enum FriendType
            {
                OWNER = 1,
                FRIEND,
                GUEST
            }
        }
    }
}