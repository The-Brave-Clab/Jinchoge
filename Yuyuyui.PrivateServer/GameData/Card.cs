﻿namespace Yuyuyui.PrivateServer
{
    public class Card : BaseUserData<Card>
    {
        public long id { get; set; } // 8 digits
        public int master_id { get; set; } // from master_data
        public int level { get; set; }
        public int exp { get; set; }
        public int potential { get; set; }
        public int hit_point { get; set; }
        public int attack { get; set; }
        public int critical { get; set; }
        public int agility { get; set; }
        public int footing_point { get; set; } // weight in database
        public long? active_skill_id { get; set; } = null;
        public long? support_skill_id { get; set; } = null;
        public long? leader_skill_id { get; set; } = null;
        public int active_skill_level { get; set; }
        public int support_skill_level { get; set; }
        public int evolution_level { get; set; }
        public int cost { get; set; }
        public int support_point { get; set; }
        public float exchange_point_rate { get; set; } // 0.0, 1.0, 2.0, 3.0, 5.0
                                                       // for different level of Taisha Point bonus
                                                       // Maybe consider changing getter/setter?

        private static long GetID()
        {
            long new_id = long.Parse(Utils.GenerateRandomDigit(8));
            while (Exists($"{new_id}"))
            {
                new_id = long.Parse(Utils.GenerateRandomDigit(8));
            }

            return new_id;
        }

        public static Card DefaultYuuna()
        {
            return new Card
            {
                id = GetID(),
                master_id = 100011,
                level = 15,
                exp = 10000,
                potential = 0,
                hit_point = 2070,
                attack = 2490,
                critical = 90,
                agility = 110,
                footing_point = 1320,
                active_skill_id = 111110000,
                support_skill_id = 132011000,
                leader_skill_id = null,
                active_skill_level = 1,
                support_skill_level = 1,
                evolution_level = 2,
                cost = 16,
                support_point = 29,
                exchange_point_rate = 0.0f
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
                hit_point = 480,
                attack = 2400,
                critical = 150,
                agility = 40,
                footing_point = 200,
                active_skill_id = 111111000,
                support_skill_id = 132012000,
                leader_skill_id = null,
                active_skill_level = 1,
                support_skill_level = 1,
                evolution_level = 1,
                cost = 23,
                support_point = 20,
                exchange_point_rate = 0.0f
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
                hit_point = 1680,
                attack = 3600,
                critical = 400,
                agility = 80,
                footing_point = 2400,
                active_skill_id = 111112000,
                support_skill_id = 132013000,
                leader_skill_id = null,
                active_skill_level = 1,
                support_skill_level = 1,
                evolution_level = 1,
                cost = 21,
                support_point = 18,
                exchange_point_rate = 0.0f
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
                hit_point = 600,
                attack = 960,
                critical = 25,
                agility = 120,
                footing_point = 300,
                active_skill_id = 111113000,
                support_skill_id = 132014000,
                leader_skill_id = null,
                active_skill_level = 1,
                support_skill_level = 1,
                evolution_level = 1,
                cost = 15,
                support_point = 23,
                exchange_point_rate = 0.0f
            };
        }

        protected override string Identifier => $"{id}";
    }
}