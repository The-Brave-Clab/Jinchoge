﻿using System.Collections.Generic;

namespace Yuyuyui.PrivateServer
{
    public class Deck : BasePlayerData<Deck, long>
    {
        public long id { get; set; }
        public long leaderUnitID { get; set; } // id of Unit (CardWithSupport) (TODO: Can be removed?)
        public string? name { get; set; } = null;
        public IList<long> units { get; set; } = new List<long>(); // id of Unit (CardWithSupport)
        
        public static long GetID()
        {
            long new_id = long.Parse(Utils.RandomStrFromChar("123456789", 1) + Utils.GenerateRandomDigit(8));
            while (Exists(new_id))
            {
                new_id = long.Parse(Utils.RandomStrFromChar("123456789", 1) + Utils.GenerateRandomDigit(8));
            }

            return new_id;
        }

        protected override long Identifier => id;
    }
}