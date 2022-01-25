namespace Yuyuyui.PrivateServer;

public class Unit : BasePlayerData<Unit, long>
{
    public long id { get; set; } // This is NOT the card ID!
    public long? baseCardID { get; set; } = null; // This IS the card ID!
    public long? supportCardID { get; set; } = null; // id of support card, same for the following 2
    public long? supportCard2ID { get; set; } = null; // UR support
    public long? assistCardID { get; set; } = null; // Miko
    public IList<long> accessories { get; set; } = new List<long>(); // Seirei ID
    protected override long Identifier => id;

    public static long GetID()
    {
        long new_id = long.Parse(Utils.GenerateRandomDigit(9));
        while (Exists(new_id))
        {
            new_id = long.Parse(Utils.GenerateRandomDigit(9));
        }

        return new_id;
    }

    public Card? GetCard()
    {
        return baseCardID == null ? null : Card.Load((long) baseCardID);
    }

    public static Unit CreateEmptyUnit()
    {
        return new()
        {
            id = GetID(),
            baseCardID = null,
            supportCardID = null,
            supportCard2ID = null,
            assistCardID = null,
            accessories = new List<long>(),
        };
    }

    public int GetHP()
    {
        if (baseCardID == null) return 0;
        // TODO <UnitCalculation>
        int hp = 0;
        hp += GetCard()!.GetHitPoint();

        if (supportCardID != null)
            hp += Support()!.GetHitPoint();

        if (supportCard2ID != null)
            hp += Support2()!.GetHitPoint();

        if (assistCardID != null)
            hp += Assist()!.GetHitPoint();

        return hp;
    }

    public int GetAtk()
    {
        if (baseCardID == null) return 0;
        // TODO <UnitCalculation>
        int atk = 0;
        atk += GetCard()!.GetAttack();

        if (supportCardID != null)
            atk += Support()!.GetAttack();

        if (supportCard2ID != null)
            atk += Support2()!.GetAttack();

        if (assistCardID != null)
            atk += Assist()!.GetAttack();

        return atk;
    }

    public long? GetMasterId()
    {
        if (baseCardID == null) return null;
        return Card.Load((long) baseCardID).master_id;
    }

    public int? GetPotential()
    {
        if (baseCardID == null) return null;
        return Card.Load((long) baseCardID).potential;
    }

    public int? GetEvolutionLevel()
    {
        if (baseCardID == null) return null;
        return Card.Load((long) baseCardID).evolution_level;
    }

    public int? GetLevel()
    {
        if (baseCardID == null) return null;
        return Card.Load((long) baseCardID).level;
    }

    public Card? Support()
    {
        return supportCardID == null ? null : Card.Load((long) supportCardID);
    }

    public Card? Support2()
    {
        return supportCard2ID == null ? null : Card.Load((long) supportCard2ID);
    }

    public Card? Assist()
    {
        return assistCardID == null ? null : Card.Load((long) assistCardID);
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
        public long? master_id { get; set; }
        public int? potential { get; set; }
        public int? evolution_level { get; set; }
        public int? level { get; set; }

        public static implicit operator CardWithSupport?(Unit? unit)
        {
            if (unit == null) return null;
            return new CardWithSupport
            {
                id = unit.id,
                hit_point = unit.GetHP(),
                attack = unit.GetAtk(),
                user_card_id = unit.baseCardID,
                support = unit.Support()?.AsSupport(),
                support_2 = unit.Support2()?.AsSupport(),
                assist = unit.Assist()?.AsSupport(),
                accessories = unit.accessories.Select(Accessory.Load).ToList(),
                master_id = unit.GetMasterId(),
                potential = unit.GetPotential(),
                evolution_level = unit.GetEvolutionLevel(),
                level = unit.GetLevel()
            };
        }

        public static implicit operator Unit?(CardWithSupport? sc)
        {
            if (sc == null) return null;
            return new Unit
            {
                id = sc.id,
                baseCardID = sc.user_card_id,
                supportCardID = ((SupportCard?) sc.support)!.user_card_id,
                supportCard2ID = ((SupportCard?) sc.support_2)!.user_card_id,
                assistCardID = ((SupportCard?) sc.assist)!.user_card_id,
                accessories = sc.accessories.Select(a => a.id).ToList(),
            };
        }
    }
}