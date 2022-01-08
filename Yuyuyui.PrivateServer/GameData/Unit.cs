namespace Yuyuyui.PrivateServer;

public class Unit : BaseUserData<Unit>
{
    public long id { get; set; } // This is NOT the card ID!
    public int hitPoint { get; set; } // TODO: Can be removed?
    public int attack { get; set; } // TODO: Can be removed?
    public long? baseCardID { get; set; } = null; // This IS the card ID!
    public long? supportCardID { get; set; } = null; // id of support card, same for the following 2
    public long? supportCard2ID { get; set; } = null; // UR support?
    public long? assistCardID { get; set; } = null; // Miko?
    public IList<long> accessories { get; set; } = new List<long>(); // Seirei ID
    public int? master_id { get; set; } = null; // TODO: Can be removed?
    public int? potential { get; set; } = null; // TODO: Can be removed?
    public int? evolutionLevel { get; set; } = null; // TODO: Can be removed?
    public int? level { get; set; } = null; // TODO: Can be removed?
    protected override string Identifier => $"{id}";

    public static long GetID()
    {
        long new_id = long.Parse(Utils.GenerateRandomDigit(9));
        while (Exists($"{new_id}"))
        {
            new_id = long.Parse(Utils.GenerateRandomDigit(9));
        }

        return new_id;
    }

    public Card GetCard()
    {
        return Card.Load($"{baseCardID}");
    }

    public static Unit CreateEmptyUnit()
    {
        return new()
        {
            id = GetID(),
            hitPoint = 0,
            attack = 0,
            baseCardID = null,
            supportCardID = null,
            supportCard2ID = null,
            assistCardID = null,
            accessories = new List<long>(),
            master_id = null,
            potential = null,
            evolutionLevel = null,
            level = null
        };
    }

    public Card? Support()
    {
        if (supportCardID == null) return null;
        return Card.Load($"{supportCardID}");
    }

    public Card? Support2()
    {
        if (supportCard2ID == null) return null;
        return Card.Load($"{supportCard2ID}");
    }

    public Card? Assist()
    {
        if (assistCardID == null) return null;
        return Card.Load($"{assistCardID}");
    }

    // This is used for JSON response
    public class CardWithSupport
    {
        public long id { get; set; }
        public int hit_point { get; set; }
        public int attack { get; set; }
        public long? user_card_id { get; set; }
        public Dictionary<string, long> support { get; set; } = new();
        public Dictionary<string, long> support_2 { get; set; } = new();
        public Dictionary<string, long> assist { get; set; } = new();
        public IList<Accessory> accessories { get; set; } = new List<Accessory>();
        public int? master_id { get; set; }
        public int? potential { get; set; }
        public int? evolution_level { get; set; }
        public int? level { get; set; }

        public static implicit operator CardWithSupport?(Unit? unit)
        {
            if (unit == null) return null;
            return new CardWithSupport
            {
                id = unit.id,
                hit_point = unit.hitPoint,
                attack = unit.attack,
                user_card_id = unit.baseCardID,
                support = unit.Support()?.AsSupport(),
                support_2 = unit.Support2()?.AsSupport(),
                assist = unit.Assist()?.AsSupport(),
                accessories = unit.accessories.Select(id => Accessory.Load($"{id}")).ToList(),
                master_id = unit.master_id,
                potential = unit.potential,
                evolution_level = unit.evolutionLevel,
                level = unit.level
            };
        }

        public static implicit operator Unit?(CardWithSupport? sc)
        {
            if (sc == null) return null;
            return new Unit
            {
                id = sc.id,
                hitPoint = sc.hit_point,
                attack = sc.attack,
                baseCardID = sc.user_card_id,
                supportCardID = ((SupportCard?) sc.support)!.user_card_id,
                supportCard2ID = ((SupportCard?) sc.support_2)!.user_card_id,
                assistCardID = ((SupportCard?) sc.assist)!.user_card_id,
                accessories = sc.accessories.Select(a => a.id).ToList(),
                master_id = sc.master_id,
                potential = sc.potential,
                evolutionLevel = sc.evolution_level,
                level = sc.level
            };
        }
    }
}