using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class Card : BasePlayerData<Card, long>
    {
        public long id { get; set; } // 8 digits
        public long master_id { get; set; } // from master_data
        public int level { get; set; }
        public int exp { get; set; }
        public int potential { get; set; }
        public int active_skill_level { get; set; }
        public int support_skill_level { get; set; }
        public int evolution_level { get; set; }

        // for different level of Taisha Point bonus
        // Maybe consider changing getter/setter?
        //public float exchange_point_rate { get; set; } // 0.0, 1.0, 2.0, 3.0, 5.0

        private static long GetID()
        {
            long new_id = long.Parse(Utils.GenerateRandomDigit(9));
            while (Exists(new_id))
            {
                new_id = long.Parse(Utils.GenerateRandomDigit(9));
            }

            return new_id;
        }

        public static Card DefaultYuuna()
        {
            return new Card
            {
                id = GetID(),
                master_id = 100011, // Consider getting this from the database
                level = 15,
                exp = 10000,
                potential = 0,
                active_skill_level = 1,
                support_skill_level = 1,
                evolution_level = 2
            };
        }

        public static Card DefaultTougou()
        {
            return new Card
            {
                id = GetID(),
                master_id = 100020,
                level = 1,
                exp = 0,
                potential = 0,
                active_skill_level = 1,
                support_skill_level = 1,
                evolution_level = 1
            };
        }

        public static Card DefaultFuu()
        {
            return new Card
            {
                id = GetID(),
                master_id = 100040,
                level = 1,
                exp = 0,
                potential = 0,
                active_skill_level = 1,
                support_skill_level = 1,
                evolution_level = 1
            };
        }

        public static Card DefaultItsuki()
        {
            return new Card
            {
                id = GetID(),
                master_id = 100050,
                level = 1,
                exp = 0,
                potential = 0,
                active_skill_level = 1,
                support_skill_level = 1,
                evolution_level = 1
            };
        }

        protected override long Identifier => id;

        public SupportCard AsSupport()
        {
            return new SupportCard
            {
                user_card_id = id,
            };
        }

        public Unit CreateUnit(
            SupportCard? support = null,
            SupportCard? support2 = null,
            SupportCard? assist = null,
            Accessory? accessory0 = null,
            Accessory? accessory1 = null)
        {
            Unit unit = new Unit
            {
                id = Unit.GetID(),
                baseCardID = id,
                supportCardID = support?.user_card_id,
                supportCard2ID = support2?.user_card_id,
                assistCardID = assist?.user_card_id,
                accessories = new List<long>(),
            };

            if (accessory0 != null)
                unit.accessories.Add(accessory0.id);
            if (accessory1 != null)
                unit.accessories.Add(accessory1.id);

            return unit;
        }

        public DataModel.Card MasterData()
        {
            using var cardDb = new CardsContext();
            return cardDb.Cards.First(c => c.Id == master_id);
        }

        public int GetHitPoint()
        {
            DataModel.Card masterCard = MasterData();
            float growthValue = GrowthKind.GetValue(masterCard.GrowthKind);
            return CalcUtil.CalcHitPointByLevel(
                level, masterCard.MinLevel, masterCard.MaxLevel,
                masterCard.MinHitPoint, masterCard.MaxHitPoint, growthValue,
                potential, masterCard.LevelMaxHitPointBonus, masterCard.PotentialHitPointArgument);
        }

        public int GetAttack()
        {
            DataModel.Card masterCard = MasterData();
            float growthValue = GrowthKind.GetValue(masterCard.GrowthKind);
            return CalcUtil.CalcAttackByLevel(
                level, masterCard.MinLevel, masterCard.MaxLevel,
                masterCard.MinAttack, masterCard.MaxAttack, growthValue,
                potential, masterCard.LevelMaxAttackBonus, masterCard.PotentialAttackArgument);
        }

        // TODO: Fill this
        public float GetExchangePointRate()
        {
            //DataModel.Card masterCard = MasterData();
            return 0.0f;
        }
    }

    public class SupportCard
    {
        public long user_card_id { get; set; } // card id

        public Card GetCard()
        {
            return Card.Load(user_card_id);
        }

        public static implicit operator Dictionary<string, long>(SupportCard? sc)
        {
            if (sc == null) return new Dictionary<string, long>();
            
            Card userCard = sc.GetCard();
            DataModel.Card masterCard = userCard.MasterData();
            float growthValue = GrowthKind.GetValue(masterCard.GrowthKind);
            
            return new Dictionary<string, long>
            {
                {"hit_point", CalcUtil.CalcHitPointByLevel(
                    userCard.level, masterCard.MinLevel, masterCard.MaxLevel, 
                    masterCard.MinHitPoint, masterCard.MaxHitPoint, growthValue, 
                    userCard.potential, masterCard.LevelMaxHitPointBonus, masterCard.PotentialHitPointArgument)},
                {"attack", CalcUtil.CalcAttackByLevel(
                    userCard.level, masterCard.MinLevel, masterCard.MaxLevel,
                    masterCard.MinAttack, masterCard.MaxAttack, growthValue, 
                    userCard.potential, masterCard.LevelMaxAttackBonus, masterCard.PotentialAttackArgument)},
                {"user_card_id", userCard.id},
                {"master_id", userCard.master_id},
                {"potential", userCard.potential},
                {"evolution_level", userCard.evolution_level},
                {"level", userCard.level}
            };
        }

        public static implicit operator SupportCard?(Dictionary<string, long> dic)
        {
            if (dic.Count == 0) return null;

            try
            {
                return new()
                {
                    user_card_id = dic["user_card_id"],
                };
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }
    }
}