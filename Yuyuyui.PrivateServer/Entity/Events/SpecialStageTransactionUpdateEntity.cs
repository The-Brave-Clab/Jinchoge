using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Yuyuyui.PrivateServer.DataModel;
using Yuyuyui.PrivateServer.DataModel.Data;

namespace Yuyuyui.PrivateServer.Entity.Events;

public class SpecialStageTransactionUpdateEntity : BaseEntity<SpecialStageTransactionUpdateEntity>
{
    private const int ATK_DIVISION = 8;
    private const float MOVEMENT_SPEED_DIVISION = 40f;
    
    public SpecialStageTransactionUpdateEntity(
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
        
        long transactionId = long.Parse(GetPathParameter("transactionId"));
        QuestTransaction transaction = QuestTransaction.Load(transactionId);

        var cardsDb = new CardsContext();
        var accessoryContext = new AccessoriesContext();
        
        long currentDeckId = transaction.createdWith.using_deck_id.GetValueOrDefault(0);
        var currentDeck = Deck.Load(currentDeckId);

        List<BattleCardData> battleCardDataList = new List<BattleCardData>();
        
        var leaderUnit = Unit.Load(currentDeck.units[0]);
        var leaderCard = Card.Load(leaderUnit.baseCardID.GetValueOrDefault(0));
        battleCardDataList.Add(CreateBattleCardData(leaderCard, leaderUnit, true, 1, cardsDb, accessoryContext));

        for (int i = 1; i < currentDeck.units.Count; i++)
        {
            var subUnit = Unit.Load(currentDeck.units[i]);
            var subCard = Card.Load(subUnit.baseCardID.GetValueOrDefault(0));
            battleCardDataList.Add(CreateBattleCardData(subCard, subUnit, false, (i + 1), cardsDb, accessoryContext));
        }
        
        var battleDeck = new BattleDeck() { cards = battleCardDataList.ToArray() };

        /* Leader Skill only exist when SR or Above */
        if (leaderCard.MasterData(cardsDb).Rarity > 200)
        {
            battleDeck.leader_skill = new LeaderSkillInfo()
            {
                id = leaderCard.MasterData(cardsDb).LeaderSkillId.GetValueOrDefault(0),
            };
        }

        var serializedStr = JsonConvert.SerializeObject(battleDeck);
        serializedStr = serializedStr.Replace("null", "{}");

        // TODO: Switch to dynamic filename
        var loadedTemplateJson = File.ReadAllText(@"C:\YuYuYuiBattleData\159130001.json");
        loadedTemplateJson = loadedTemplateJson.Replace("$template", serializedStr);

        responseBody = Encoding.UTF8.GetBytes(loadedTemplateJson);
        SetBasicResponseHeaders();

        return Task.CompletedTask;
    }

    private BattleCardData CreateBattleCardData(Card leaderCard, Unit leaderUnit, bool isLeader, int order, CardsContext cardsDb, AccessoriesContext accessoryContext)
    {
        BattleCardBaseInfo leaderBattleCardBaseInfo = CreateBattleCardBaseInfo(leaderCard, cardsDb);
        List<AccessoryInfo> leaderAccessoryInfoList = CreateAccessoryInfoList(leaderUnit, accessoryContext);
        BattleCardData leaderBattleCardData = CreateBattleCardData(
            leaderCard,
            isLeader,
            order,
            cardsDb,
            leaderBattleCardBaseInfo,
            leaderAccessoryInfoList
        );

        var unitHp = leaderCard.GetHitPoint(cardsDb);

        /* Basic Support Unit */
        if (leaderUnit.supportCardID != null)
        {
            var supportCard1 = Card.Load(leaderUnit.supportCardID.GetValueOrDefault(0));
            var supportCard1BaseInfo = CreateBattleCardBaseInfo(supportCard1, cardsDb);

            leaderBattleCardData.supporter = CreateSupporterCardData(supportCard1BaseInfo, supportCard1, cardsDb);
            unitHp += supportCard1.GetHitPoint(cardsDb);
        }

        /* UR Slot */
        if (leaderUnit.supportCard2ID != null)
        {
            var supportCard2 = Card.Load(leaderUnit.supportCard2ID.GetValueOrDefault(0));
            var supportCard2BaseInfo = CreateBattleCardBaseInfo(supportCard2, cardsDb);

            leaderBattleCardData.supporter_2 = CreateSupporterCardData(supportCard2BaseInfo, supportCard2, cardsDb);
        }

        /* Miko Slot if any */
        if (isLeader && leaderUnit.assistCardID != null)
        {
            var mikoCard = Card.Load(leaderUnit.assistCardID.GetValueOrDefault(0));
            var mikoCardBaseInfo = CreateBattleCardBaseInfo(mikoCard, cardsDb);

            leaderBattleCardData.assist = CreateSupporterCardData(mikoCardBaseInfo, mikoCard, cardsDb);
            leaderBattleCardData.base_info.attack += mikoCard.GetAttack(cardsDb);
            unitHp += mikoCard.GetHitPoint(cardsDb);
        }

        leaderBattleCardData.hp = unitHp;
        return leaderBattleCardData;
    }

    private SupporterCardData CreateSupporterCardData(BattleCardBaseInfo supportCardBaseInfo, Card supportCard, CardsContext cardsDb)
    {
        DataModel.Card cardMasterData = supportCard.MasterData(cardsDb);
        List<PassiveSkillInfo> passiveSkillInfos = new List<PassiveSkillInfo>();

        PassiveSkillInfo passiveSkillInfo1 = new PassiveSkillInfo()
        {
            id = supportCard.MasterData(cardsDb).SupportSkill1Id.GetValueOrDefault(0),
            level = supportCard.support_skill_level
        };
        passiveSkillInfos.Add(passiveSkillInfo1);

        /* Additional Skill for UR Miko */
        if (cardMasterData.SupportSkill2Id != null)
        {
            PassiveSkillInfo passiveSkillInfo2 = new PassiveSkillInfo()
            {
                id = supportCard.MasterData(cardsDb).SupportSkill2Id.GetValueOrDefault(0),
                level = supportCard.support_skill_level
            };
            passiveSkillInfos.Add(passiveSkillInfo2);
        }
        
        return new SupporterCardData()
        {
            base_info = supportCardBaseInfo,
            passive_skills = passiveSkillInfos.ToArray()
        };
    }

    private BattleCardData CreateBattleCardData(Card card, bool isLeader, int order, CardsContext cardsDb, BattleCardBaseInfo leaderBattleCardBaseInfo, List<AccessoryInfo> leaderAccessoryInfoList)
    {
        return new BattleCardData()
        {
            id = card.id,
            leader = isLeader,
            friend_type = 1,
            order = order,
            active_skill = new ActiveSkillInfo()
            {
                id = card.MasterData(cardsDb).ActiveSkillId.GetValueOrDefault(0),
                level = card.active_skill_level
            },
            passive_skills = new []
            {
                new PassiveSkillInfo()
                {
                    id = card.MasterData(cardsDb).SupportSkill1Id.GetValueOrDefault(0),
                    level = card.support_skill_level
                }
            },
            base_info = leaderBattleCardBaseInfo,
            accessories = leaderAccessoryInfoList.ToArray()
        };
    }

    private BattleCardBaseInfo CreateBattleCardBaseInfo(Card leaderCard, CardsContext cardsDb)
    {
        return new BattleCardBaseInfo()
        {
            element = leaderCard.MasterData(cardsDb).Element,
            character_type = leaderCard.MasterData(cardsDb).CharacterId,
            character_voice_type = leaderCard.MasterData(cardsDb).CharacterId,
            attack_radius = DetermineRadius(leaderCard.MasterData(cardsDb).AttackType),
            defense = 1,
            radius = 0.6f,
            attack_type = leaderCard.MasterData(cardsDb).AttackType,
            master_id = leaderCard.master_id,
            attack = leaderCard.GetAttack(cardsDb) / ATK_DIVISION,
            avoid_rate = 0,
            different_line_attack = leaderCard.MasterData(cardsDb).AttackType == 2,
            move_speed = leaderCard.MasterData(cardsDb).MaxAgility / MOVEMENT_SPEED_DIVISION,
            hit_rate = 1.0f,
            attack_pace = leaderCard.MasterData(cardsDb).AttackPace,
            critical_point = leaderCard.MasterData(cardsDb).MaxCritical,
            footing_point = leaderCard.MasterData(cardsDb).MaxWeight
        };
    }

    private List<AccessoryInfo> CreateAccessoryInfoList(Unit unit, AccessoriesContext accessoriesContext)
    {
        List<AccessoryInfo> accessoryInfoList = new List<AccessoryInfo>();
        foreach (var accessory in unit.accessories)
        {
            Accessory userAccessory = Accessory.Load(accessory);
            AccessoryInfo accessoryInfo = new AccessoryInfo()
            {
                master_id = userAccessory.master_id,
                passive_skill = new PassiveSkillInfo()
                {
                    id = accessoriesContext.Accessories.First(accessories => accessories.Id == userAccessory.master_id).SkillId,
                    level = userAccessory.level
                }
            };
            accessoryInfoList.Add(accessoryInfo);
        }

        return accessoryInfoList;
    }
    
    private float DetermineRadius(int attackType)
    {
        if (attackType == 1)
        {
            return 1.0f;
        }
        if (attackType == 2)
        {
            return 1;
        }
        if (attackType == 3)
        {
            return 10000.0f;
        }

        return 1;
    }

    public string createResponseBody()
    {
        string bossPartStr =
            "{\"id\":\"B10002\",\"drop_box_type\":0,\"pop_type\":1,\"pop_value\":24,\"pop_cond_type\":0,\"pop_cond_value\":0,\"line_num\":1,\"enemy_id\":\"B10002\",\"statuses\":[{\"element\":3,\"battle_level\":5,\"atk_level\":1,\"hp_level\":1,\"enemy\":{\"id\":\"B10002\",\"enemy_type\":1501,\"image_scale\":1.024,\"radius\":1,\"attack\":177,\"hp\":19952,\"attack_radius\":1,\"attack_pace\":2,\"move_speed\":2.5,\"defense\":0,\"avoid_rate\":0,\"hit_rate\":1,\"critical_point\":59,\"footing_point\":33200,\"exp\":160,\"exp_per_attack\":2,\"size_type\":0,\"move_type\":0,\"move_to_x\":0,\"wait_time\":0,\"move_time\":0,\"lc_move_type\":3,\"lc_trigger_type\":2,\"lc_max_count\":3,\"lc_interval\":2,\"battle_item_id\":0,\"battle_item_drop_rate\":0,\"notice\":true,\"name\":\"アタッカ\",\"vertex\":true,\"attack_effect_size\":3,\"hp_gauge_type\":3,\"as_first_interval\":0,\"hit_effect_height\":0.4,\"active_skills\":[],\"passive_skills\":[{\"id\":501000000,\"level\":1}],\"character_type\":null,\"master_id\":null,\"summon_size_type\":null},\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}}]}";
        string overallStr =
            "{\"boss\":" + bossPartStr + ",\"chapter\":{\"id\":3298167,\"master_id\":1,\"kind\":0,\"start_at\":0,\"end_at\":0,\"detail_url\":\"https://article.yuyuyui.jp/article/episodes/1\",\"stack_point\":0,\"locked\":false,\"new_released\":false,\"completed\":false,\"available_user_level\":0},\"episode\":{\"id\":6373832,\"master_id\":1001,\"finish\":false,\"detail_url\":\"https://article.yuyuyui.jp/article/episodes/1001\"},\"stage\":{\"id\":48021227,\"master_id\":10010102,\"finish\":true,\"score_finished_count\":3,\"locked\":false,\"campaign_exchange_point_rate\":1,\"campaign_stamina_rate\":1,\"end_at_stamina_campaign\":null,\"stage_by_level_end_at\":1869663600,\"play_auto_clear\":false,\"no_friend\":null},\"battle_info\":{\"score_speed_expedition_second\":71,\"loop_start_time\":67,\"enemy_limit\":5},\"deck\":{\"leader_skill\":{},\"friend_leader_skill\":{},\"cards\":[{\"id\":114769472,\"base_info\":{\"element\":1,\"move_speed\":3.6300000000000003,\"avoid_rate\":0,\"attack\":662,\"hit_rate\":1,\"attack_radius\":1,\"different_line_attack\":false,\"defense\":1,\"critical_point\":90,\"footing_point\":1320,\"attack_pace\":1,\"radius\":0.6,\"character_type\":1,\"master_id\":100011,\"attack_type\":1,\"character_voice_type\":1},\"passive_skills\":[{\"id\":132011000,\"level\":1}],\"active_skill\":{\"id\":111110000,\"level\":1},\"accessories\":[],\"supporter\":{\"base_info\":{\"element\":2,\"move_speed\":1.32,\"avoid_rate\":0,\"attack\":629,\"hit_rate\":1,\"attack_radius\":10000,\"different_line_attack\":false,\"defense\":1,\"critical_point\":150,\"footing_point\":200,\"attack_pace\":2,\"radius\":0.6,\"character_type\":2,\"master_id\":100020,\"attack_type\":3,\"character_voice_type\":2},\"passive_skills\":[{\"id\":132012000,\"level\":1}]},\"supporter_2\":{},\"order\":1,\"friend_type\":1,\"leader\":true,\"hp\":3069,\"assist\":{}},{\"id\":114769473,\"base_info\":{\"element\":4,\"move_speed\":2.64,\"avoid_rate\":0,\"attack\":766,\"hit_rate\":1,\"attack_radius\":1,\"different_line_attack\":false,\"defense\":1,\"critical_point\":400,\"footing_point\":2400,\"attack_pace\":2.4,\"radius\":0.6,\"character_type\":3,\"master_id\":100040,\"attack_type\":1,\"character_voice_type\":3},\"passive_skills\":[{\"id\":132013000,\"level\":1}],\"active_skill\":{\"id\":111112000,\"level\":1},\"accessories\":[],\"supporter\":{},\"supporter_2\":{},\"order\":2,\"friend_type\":1,\"leader\":false,\"hp\":1787,\"assist\":{}},{\"id\":114769474,\"base_info\":{\"element\":3,\"move_speed\":3.96,\"avoid_rate\":0,\"attack\":200,\"hit_rate\":1,\"attack_radius\":2,\"different_line_attack\":true,\"defense\":1,\"critical_point\":25,\"footing_point\":300,\"attack_pace\":0.9,\"radius\":0.6,\"character_type\":4,\"master_id\":100050,\"attack_type\":2,\"character_voice_type\":4},\"passive_skills\":[{\"id\":132014000,\"level\":1}],\"active_skill\":{\"id\":111113000,\"level\":1},\"accessories\":[],\"supporter\":{},\"supporter_2\":{},\"order\":3,\"friend_type\":1,\"leader\":false,\"hp\":626,\"assist\":{}},{\"id\":111067718,\"base_info\":{\"element\":3,\"move_speed\":4.29,\"avoid_rate\":0,\"attack\":3882,\"hit_rate\":1,\"attack_radius\":1,\"different_line_attack\":false,\"defense\":1,\"critical_point\":180,\"footing_point\":1440,\"attack_pace\":1,\"radius\":0.6,\"character_type\":1,\"master_id\":301113,\"attack_type\":1,\"character_voice_type\":1},\"passive_skills\":[{\"id\":132011032,\"level\":2}],\"active_skill\":{\"id\":111110032,\"level\":1},\"accessories\":[{\"master_id\":500118,\"passive_skill\":{\"id\":141104202,\"level\":1}},{\"master_id\":500219,\"passive_skill\":{\"id\":141104904,\"level\":1}}],\"supporter\":{\"base_info\":{\"element\":3,\"move_speed\":2.64,\"avoid_rate\":0,\"attack\":1918,\"hit_rate\":1,\"attack_radius\":1,\"different_line_attack\":false,\"defense\":1,\"critical_point\":240,\"footing_point\":3000,\"attack_pace\":2.4,\"radius\":0.6,\"character_type\":3,\"master_id\":200860,\"attack_type\":1,\"character_voice_type\":3},\"passive_skills\":[{\"id\":132013005,\"level\":1}]},\"supporter_2\":{},\"order\":4,\"friend_type\":3,\"leader\":false,\"hp\":18712,\"assist\":{\"base_info\":{\"element\":3,\"move_speed\":0,\"avoid_rate\":0,\"attack\":108,\"hit_rate\":1,\"attack_radius\":1,\"different_line_attack\":false,\"defense\":1,\"critical_point\":0,\"footing_point\":0,\"attack_pace\":1,\"radius\":0.6,\"character_type\":19,\"master_id\":301070,\"attack_type\":4,\"character_voice_type\":19},\"passive_skills\":[{\"id\":132029004,\"level\":1}]}}],\"stage_leader_skills\":[]},\"tree_hp\":3,\"brave_systems\":[],\"enemies\":[{\"id\":\"1\",\"enemy_type\":1001,\"image_scale\":0.7,\"radius\":1,\"attack\":960,\"hp\":10800,\"attack_radius\":1,\"attack_pace\":2,\"move_speed\":1.4,\"defense\":0,\"avoid_rate\":0,\"hit_rate\":1,\"critical_point\":10,\"footing_point\":800,\"exp\":10,\"exp_per_attack\":0,\"size_type\":0,\"move_type\":0,\"move_to_x\":0,\"wait_time\":0,\"move_time\":0,\"lc_move_type\":0,\"lc_trigger_type\":0,\"lc_max_count\":0,\"lc_interval\":0,\"battle_item_id\":1,\"battle_item_drop_rate\":0.1,\"notice\":false,\"name\":\"星屑\",\"vertex\":true,\"attack_effect_size\":1,\"hp_gauge_type\":1,\"as_first_interval\":0,\"hit_effect_height\":0.4,\"active_skills\":[],\"passive_skills\":[],\"character_type\":null,\"master_id\":null,\"summon_size_type\":null},{\"id\":\"2\",\"enemy_type\":1001,\"image_scale\":0.715,\"radius\":1,\"attack\":1056,\"hp\":12960,\"attack_radius\":1,\"attack_pace\":2,\"move_speed\":1.45,\"defense\":0,\"avoid_rate\":0,\"hit_rate\":1,\"critical_point\":15,\"footing_point\":900,\"exp\":10,\"exp_per_attack\":0,\"size_type\":0,\"move_type\":0,\"move_to_x\":0,\"wait_time\":0,\"move_time\":0,\"lc_move_type\":0,\"lc_trigger_type\":0,\"lc_max_count\":0,\"lc_interval\":0,\"battle_item_id\":1,\"battle_item_drop_rate\":0.125,\"notice\":false,\"name\":\"星屑\",\"vertex\":true,\"attack_effect_size\":1,\"hp_gauge_type\":1,\"as_first_interval\":0,\"hit_effect_height\":0.4,\"active_skills\":[],\"passive_skills\":[],\"character_type\":null,\"master_id\":null,\"summon_size_type\":null},{\"id\":\"3\",\"enemy_type\":1001,\"image_scale\":0.73,\"radius\":1,\"attack\":1152,\"hp\":15552,\"attack_radius\":1,\"attack_pace\":2,\"move_speed\":1.5,\"defense\":0,\"avoid_rate\":0,\"hit_rate\":1,\"critical_point\":20,\"footing_point\":1000,\"exp\":10,\"exp_per_attack\":0,\"size_type\":0,\"move_type\":0,\"move_to_x\":0,\"wait_time\":0,\"move_time\":0,\"lc_move_type\":0,\"lc_trigger_type\":0,\"lc_max_count\":0,\"lc_interval\":0,\"battle_item_id\":1,\"battle_item_drop_rate\":0.15,\"notice\":false,\"name\":\"星屑\",\"vertex\":true,\"attack_effect_size\":1,\"hp_gauge_type\":1,\"as_first_interval\":0,\"hit_effect_height\":0.4,\"active_skills\":[],\"passive_skills\":[],\"character_type\":null,\"master_id\":null,\"summon_size_type\":null},{\"id\":\"4\",\"enemy_type\":1001,\"image_scale\":0.745,\"radius\":1,\"attack\":1248,\"hp\":18662,\"attack_radius\":1,\"attack_pace\":2,\"move_speed\":1.55,\"defense\":0,\"avoid_rate\":0,\"hit_rate\":1,\"critical_point\":25,\"footing_point\":1100,\"exp\":10,\"exp_per_attack\":0,\"size_type\":0,\"move_type\":0,\"move_to_x\":0,\"wait_time\":0,\"move_time\":0,\"lc_move_type\":0,\"lc_trigger_type\":0,\"lc_max_count\":0,\"lc_interval\":0,\"battle_item_id\":1,\"battle_item_drop_rate\":0.175,\"notice\":false,\"name\":\"星屑\",\"vertex\":true,\"attack_effect_size\":1,\"hp_gauge_type\":1,\"as_first_interval\":0,\"hit_effect_height\":0.4,\"active_skills\":[],\"passive_skills\":[],\"character_type\":null,\"master_id\":null,\"summon_size_type\":null},{\"id\":\"1001\",\"enemy_type\":1501,\"image_scale\":0.64,\"radius\":1,\"attack\":1800,\"hp\":54000,\"attack_radius\":1,\"attack_pace\":2.6,\"move_speed\":2.5,\"defense\":0,\"avoid_rate\":0,\"hit_rate\":1,\"critical_point\":150,\"footing_point\":1000,\"exp\":30,\"exp_per_attack\":0,\"size_type\":0,\"move_type\":0,\"move_to_x\":0,\"wait_time\":0,\"move_time\":0,\"lc_move_type\":3,\"lc_trigger_type\":2,\"lc_max_count\":3,\"lc_interval\":5,\"battle_item_id\":0,\"battle_item_drop_rate\":0,\"notice\":true,\"name\":\"アタッカ\",\"vertex\":true,\"attack_effect_size\":2,\"hp_gauge_type\":2,\"as_first_interval\":0,\"hit_effect_height\":0.4,\"active_skills\":[],\"passive_skills\":[{\"id\":501000000,\"level\":1}],\"character_type\":null,\"master_id\":null,\"summon_size_type\":null},{\"id\":\"1002\",\"enemy_type\":1502,\"image_scale\":0.64,\"radius\":1,\"attack\":720,\"hp\":36000,\"attack_radius\":1,\"attack_pace\":2.6,\"move_speed\":1.05,\"defense\":0,\"avoid_rate\":0,\"hit_rate\":1,\"critical_point\":150,\"footing_point\":2000,\"exp\":30,\"exp_per_attack\":0,\"size_type\":0,\"move_type\":0,\"move_to_x\":0,\"wait_time\":0,\"move_time\":0,\"lc_move_type\":0,\"lc_trigger_type\":0,\"lc_max_count\":0,\"lc_interval\":0,\"battle_item_id\":1,\"battle_item_drop_rate\":0.15,\"notice\":true,\"name\":\"グリッサンド\",\"vertex\":true,\"attack_effect_size\":2,\"hp_gauge_type\":2,\"as_first_interval\":0,\"hit_effect_height\":0.4,\"active_skills\":[],\"passive_skills\":[],\"character_type\":null,\"master_id\":null,\"summon_size_type\":null}],\"wave_timelines\":[{\"id\":\"1\",\"drop_box_type\":0,\"pop_time\":0,\"line_num\":0,\"enemy_id\":\"1\",\"element\":3,\"battle_level\":1,\"atk_level\":0.045,\"hp_level\":0.045,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"2\",\"drop_box_type\":0,\"pop_time\":2.4,\"line_num\":1,\"enemy_id\":\"1\",\"element\":3,\"battle_level\":1,\"atk_level\":0.045,\"hp_level\":0.045,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"3\",\"drop_box_type\":0,\"pop_time\":4.8,\"line_num\":2,\"enemy_id\":\"1\",\"element\":3,\"battle_level\":1,\"atk_level\":0.045,\"hp_level\":0.045,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"4\",\"drop_box_type\":0,\"pop_time\":7.2,\"line_num\":1,\"enemy_id\":\"1\",\"element\":3,\"battle_level\":1,\"atk_level\":0.045,\"hp_level\":0.045,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"5\",\"drop_box_type\":0,\"pop_time\":9.6,\"line_num\":2,\"enemy_id\":\"1\",\"element\":3,\"battle_level\":1,\"atk_level\":0.045,\"hp_level\":0.045,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"6\",\"drop_box_type\":0,\"pop_time\":11.9,\"line_num\":0,\"enemy_id\":\"1\",\"element\":3,\"battle_level\":1,\"atk_level\":0.045,\"hp_level\":0.045,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"7\",\"drop_box_type\":0,\"pop_time\":14.2,\"line_num\":0,\"enemy_id\":\"2\",\"element\":3,\"battle_level\":2,\"atk_level\":0.05,\"hp_level\":0.05,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"8\",\"drop_box_type\":0,\"pop_time\":16.5,\"line_num\":0,\"enemy_id\":\"2\",\"element\":3,\"battle_level\":2,\"atk_level\":0.05,\"hp_level\":0.05,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"9\",\"drop_box_type\":1,\"pop_time\":18.8,\"line_num\":0,\"enemy_id\":\"1002\",\"element\":3,\"battle_level\":2,\"atk_level\":0.05,\"hp_level\":0.05,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"10\",\"drop_box_type\":0,\"pop_time\":21.1,\"line_num\":0,\"enemy_id\":\"1002\",\"element\":3,\"battle_level\":2,\"atk_level\":0.05,\"hp_level\":0.05,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"11\",\"drop_box_type\":0,\"pop_time\":23.4,\"line_num\":2,\"enemy_id\":\"2\",\"element\":3,\"battle_level\":2,\"atk_level\":0.05,\"hp_level\":0.05,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"12\",\"drop_box_type\":0,\"pop_time\":25.7,\"line_num\":0,\"enemy_id\":\"2\",\"element\":3,\"battle_level\":2,\"atk_level\":0.05,\"hp_level\":0.05,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"13\",\"drop_box_type\":0,\"pop_time\":28,\"line_num\":2,\"enemy_id\":\"3\",\"element\":3,\"battle_level\":3,\"atk_level\":0.054,\"hp_level\":0.054,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"14\",\"drop_box_type\":0,\"pop_time\":30.2,\"line_num\":1,\"enemy_id\":\"3\",\"element\":3,\"battle_level\":3,\"atk_level\":0.054,\"hp_level\":0.054,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"15\",\"drop_box_type\":0,\"pop_time\":32.4,\"line_num\":0,\"enemy_id\":\"3\",\"element\":3,\"battle_level\":3,\"atk_level\":0.054,\"hp_level\":0.054,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"16\",\"drop_box_type\":0,\"pop_time\":34.6,\"line_num\":1,\"enemy_id\":\"1001\",\"element\":3,\"battle_level\":3,\"atk_level\":0.054,\"hp_level\":0.054,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"17\",\"drop_box_type\":0,\"pop_time\":36.8,\"line_num\":0,\"enemy_id\":\"3\",\"element\":3,\"battle_level\":3,\"atk_level\":0.054,\"hp_level\":0.054,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"18\",\"drop_box_type\":0,\"pop_time\":39,\"line_num\":0,\"enemy_id\":\"3\",\"element\":3,\"battle_level\":3,\"atk_level\":0.054,\"hp_level\":0.054,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"19\",\"drop_box_type\":0,\"pop_time\":41.2,\"line_num\":2,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"20\",\"drop_box_type\":0,\"pop_time\":43.4,\"line_num\":2,\"enemy_id\":\"1001\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"21\",\"drop_box_type\":0,\"pop_time\":45.6,\"line_num\":0,\"enemy_id\":\"1002\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"22\",\"drop_box_type\":0,\"pop_time\":47.7,\"line_num\":0,\"enemy_id\":\"1001\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"23\",\"drop_box_type\":0,\"pop_time\":49.8,\"line_num\":0,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"24\",\"drop_box_type\":0,\"pop_time\":51.9,\"line_num\":0,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"25\",\"drop_box_type\":0,\"pop_time\":54,\"line_num\":2,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"26\",\"drop_box_type\":0,\"pop_time\":56.1,\"line_num\":1,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"27\",\"drop_box_type\":0,\"pop_time\":58.2,\"line_num\":1,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"28\",\"drop_box_type\":0,\"pop_time\":60.3,\"line_num\":2,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"29\",\"drop_box_type\":0,\"pop_time\":62.4,\"line_num\":1,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"30\",\"drop_box_type\":0,\"pop_time\":64.4,\"line_num\":0,\"enemy_id\":\"1002\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"31\",\"drop_box_type\":0,\"pop_time\":66.4,\"line_num\":2,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"32\",\"drop_box_type\":0,\"pop_time\":68.4,\"line_num\":0,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"33\",\"drop_box_type\":0,\"pop_time\":70.4,\"line_num\":0,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"34\",\"drop_box_type\":0,\"pop_time\":72.4,\"line_num\":1,\"enemy_id\":\"1002\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"35\",\"drop_box_type\":0,\"pop_time\":74.4,\"line_num\":1,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"36\",\"drop_box_type\":0,\"pop_time\":76.4,\"line_num\":0,\"enemy_id\":\"1002\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"37\",\"drop_box_type\":0,\"pop_time\":78.4,\"line_num\":1,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"38\",\"drop_box_type\":0,\"pop_time\":80.3,\"line_num\":1,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"39\",\"drop_box_type\":0,\"pop_time\":82.2,\"line_num\":1,\"enemy_id\":\"1001\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"40\",\"drop_box_type\":0,\"pop_time\":84.1,\"line_num\":2,\"enemy_id\":\"1002\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"41\",\"drop_box_type\":0,\"pop_time\":86,\"line_num\":1,\"enemy_id\":\"1002\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"42\",\"drop_box_type\":0,\"pop_time\":87.9,\"line_num\":1,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"43\",\"drop_box_type\":0,\"pop_time\":89.8,\"line_num\":0,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"44\",\"drop_box_type\":0,\"pop_time\":91.7,\"line_num\":1,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"45\",\"drop_box_type\":0,\"pop_time\":93.6,\"line_num\":2,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"46\",\"drop_box_type\":0,\"pop_time\":95.5,\"line_num\":2,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"47\",\"drop_box_type\":0,\"pop_time\":97.3,\"line_num\":2,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"48\",\"drop_box_type\":0,\"pop_time\":99.1,\"line_num\":0,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"49\",\"drop_box_type\":0,\"pop_time\":100.9,\"line_num\":1,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"50\",\"drop_box_type\":0,\"pop_time\":102.7,\"line_num\":0,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"51\",\"drop_box_type\":0,\"pop_time\":104.5,\"line_num\":2,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"52\",\"drop_box_type\":0,\"pop_time\":106.3,\"line_num\":2,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"53\",\"drop_box_type\":0,\"pop_time\":108.1,\"line_num\":2,\"enemy_id\":\"1002\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"54\",\"drop_box_type\":0,\"pop_time\":109.9,\"line_num\":0,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"55\",\"drop_box_type\":0,\"pop_time\":111.6,\"line_num\":0,\"enemy_id\":\"1002\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"56\",\"drop_box_type\":0,\"pop_time\":113.3,\"line_num\":0,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"57\",\"drop_box_type\":0,\"pop_time\":115,\"line_num\":2,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"58\",\"drop_box_type\":0,\"pop_time\":116.7,\"line_num\":2,\"enemy_id\":\"4\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}},{\"id\":\"59\",\"drop_box_type\":0,\"pop_time\":118.4,\"line_num\":2,\"enemy_id\":\"1001\",\"element\":3,\"battle_level\":4,\"atk_level\":0.06,\"hp_level\":0.06,\"attr_resistances\":{\"element_normal\":0,\"element_blue\":0,\"element_green\":0,\"element_red\":0,\"element_yellow\":0,\"element_purple\":0,\"type_short\":0,\"type_middle\":0,\"type_long\":0}}],\"game_mode_rule\":{\"id\":1,\"result_type\":1}}";

        return overallStr;
    }
}