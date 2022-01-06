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
}