using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            Response responseObj = new()
            {
                boss = new(), // TODO
                chapter = ChapterEntity.Response.Chapter.GetFromDatabase(dbChapter, player),
                episode = EpisodeEntity.Response.Episode.GetFromDatabase(dbEpisode, player),
                stage = StageEntity.Response.Stage.GetFromDatabase(dbStage, player),
                battle_info = new(), // TODO
                deck = new(), // TODO
                tree_hp = 3,
                brave_systems = new(), // TODO
                enemies = new(), // TODO
                wave_timelines = new(), // TODO
                game_mode_rule = new()
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
            public Boss? boss { get; set; } = null; // TODO
            public List<Boss>? multiple_bosses { get; set; } = null; // TODO
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
                public List<PassiveSkill> passive_skills { get; set; } = new();
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
                public List<PassiveSkill> stage_leader_skills { get; set; } = new();
                public List<BattleCardData> cards { get; set; } = new();
            }
            public class BattleCardData
            {
                public long id { get; set; }
                public CardBaseInfo base_info { get; set; } = new ();
                public SupporterCardData supporter { get; set; } = new ();
                public int order { get; set; }
                public ActiveSkill active_skill { get; set; } = new ();
                public List<Accessory> accessories { get; set; } = new ();
                public List<PassiveSkill> passive_skills { get; set; } = new ();
                public int hp { get; set; }
                public bool leader { get; set; }
                public FriendType friend_type { get; set; }
                public AssistCardData assist { get; set; } = new();
            }
            public class CardBaseInfo
            {
                public AttributeType element { get; set; }
                public int character_type { get; set; }
                public int character_voice_type { get; set; }
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
            }

            public class SupporterCardData
            {
                public CardBaseInfo base_info { get; set; } = new();
                public List<PassiveSkill> passive_skills { get; set; } = new();
            }
            
            public class AssistCardData
            {
                public CardBaseInfo base_info { get; set; } = new();
                public List<PassiveSkill> passive_skills { get; set; } = new();
            }
            
            public class Accessory
            {
                public long master_id { get; set; }
                public PassiveSkill passive_skill { get; set; } = new();
            }

            public class ActiveSkill
            {
                public long id { get; set; }
                public int level { get; set; }
                public float interval { get; set; }
                public float weight { get; set; }
            }

            public class PassiveSkill
            {
                public long id { get; set; }
                public int level { get; set; }
            }

            public class LeaderSkillInfo
            {
                public int id { get; set; }
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