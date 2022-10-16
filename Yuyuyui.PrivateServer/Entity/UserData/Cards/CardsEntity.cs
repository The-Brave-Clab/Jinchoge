using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer
{
    public class CardsEntity : BaseEntity<CardsEntity>
    {
        public CardsEntity(
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

            if (player.cards.Count == 0)
            {
                var yuuna = Yuyuyui.PrivateServer.Card.DefaultYuuna();
                var tougou = Yuyuyui.PrivateServer.Card.DefaultTougou();
                var fuu = Yuyuyui.PrivateServer.Card.DefaultFuu();
                var itsuki = Yuyuyui.PrivateServer.Card.DefaultItsuki();
                player.cards.Add(100010, yuuna.id); // since the key is base_card_id, we manually specify this
                player.cards.Add(tougou.master_id, tougou.id);
                player.cards.Add(fuu.master_id, fuu.id);
                player.cards.Add(itsuki.master_id, itsuki.id);
                yuuna.Save();
                tougou.Save();
                fuu.Save();
                itsuki.Save();
                player.Save();
                Utils.Log(Resources.LOG_PS_CARD_ASSIGN_DEFAULT);
            }

            // Utils.LogWarning("Taisha point bonus not applied!");

            using var cardsDb = new CardsContext();
            Response responseObj = new()
            {
                cards = player.cards
                    .Select(p => p.Value)
                    .ToDictionary(c => c, c => Card.FromPlayerCardData(cardsDb, c))
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IDictionary<long, Card> cards { get; set; } = new Dictionary<long, Card>();
        }

        public class Card
        {
            public long id { get; set; } // 8 digits
            public long master_id { get; set; } // from master_data
            public int level { get; set; }
            public long exp { get; set; }
            public int potential { get; set; }
            public int hit_point { get; set; }
            public int attack { get; set; }
            public int critical { get; set; }
            public int agility { get; set; }
            public int footing_point { get; set; } // weight in database
            public long? active_skill_id { get; set; }
            public long? support_skill_1_id { get; set; }
            public long? support_skill_2_id { get; set; }
            public long? leader_skill_id { get; set; }
            public int active_skill_level { get; set; }
            public int support_skill_level { get; set; }
            public int evolution_level { get; set; }
            public int cost { get; set; }
            public int support_point { get; set; }
            public float exchange_point_rate { get; set; } // 0.0, 1.0, 2.0, 3.0, 5.0

            public static Card FromPlayerCardData(CardsContext cardsDb, long userCardId)
            {
                return FromPlayerCardData(cardsDb, Yuyuyui.PrivateServer.Card.Load(userCardId));
            }

            public static Card FromPlayerCardData(CardsContext cardsDb,
                Yuyuyui.PrivateServer.Card userCard)
            {
                DataModel.Card masterCard = userCard.MasterData(cardsDb);
                float growthValue = GrowthKind.GetValue(masterCard.GrowthKind);

                return new()
                {
                    id = userCard.id,
                    master_id = userCard.master_id,
                    level = userCard.level,
                    exp = userCard.exp,
                    potential = userCard.potential,
                    hit_point = CalcUtil.CalcHitPointByLevel(
                        userCard.level, masterCard.MinLevel, masterCard.MaxLevel, 
                        masterCard.MinHitPoint, masterCard.MaxHitPoint, growthValue, 
                        userCard.potential, masterCard.LevelMaxHitPointBonus, masterCard.PotentialHitPointArgument),
                    attack = CalcUtil.CalcAttackByLevel(
                        userCard.level, masterCard.MinLevel, masterCard.MaxLevel,
                        masterCard.MinAttack, masterCard.MaxAttack, growthValue, 
                        userCard.potential, masterCard.LevelMaxAttackBonus, masterCard.PotentialAttackArgument),
                    critical = CalcUtil.CalcParamByLevel(
                        userCard.level, masterCard.MinLevel, masterCard.MaxLevel,
                        masterCard.MinCritical, masterCard.MaxCritical, growthValue),
                    agility = CalcUtil.CalcParamByLevel(
                        userCard.level, masterCard.MinLevel, masterCard.MaxLevel,
                        masterCard.MinAgility, masterCard.MaxAgility, growthValue),
                    footing_point = CalcUtil.CalcParamByLevel(
                        userCard.level, masterCard.MinLevel, masterCard.MaxLevel,
                        masterCard.MinWeight, masterCard.MaxWeight, growthValue),
                    active_skill_id = masterCard.ActiveSkillId,
                    support_skill_1_id = masterCard.SupportSkill1Id,
                    support_skill_2_id = masterCard.SupportSkill2Id,
                    leader_skill_id = masterCard.LeaderSkillId,
                    active_skill_level = userCard.active_skill_level,
                    /* Actually, Both Miko and Hero trusts support_skill_level */
                    support_skill_level = userCard.support_skill_level,
                    evolution_level = userCard.evolution_level,
                    cost = masterCard.Cost,
                    support_point = masterCard.SupportPoint + userCard.base_sp_increment,
                    exchange_point_rate = userCard.GetExchangePointRate()
                };
            }
        }
    }
}