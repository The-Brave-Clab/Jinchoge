using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer;

public class Deck : BasePlayerData<Deck, long>
{
    public long id { get; set; }
    public long leaderUnitID { get; set; } // id of Unit (CardWithSupport) (TODO: Can be removed?)
    public string? name { get; set; } = null;
    public IList<long> units { get; set; } = new List<long>(); // id of Unit (CardWithSupport)

    public const long DUMMY_DECK_ID = 1;
        
    public static async Task<long> GetID()
    {
        long new_id = long.Parse(Utils.RandomStrFromChar("123456789", 1) + Utils.GenerateRandomDigit(8));
        while (await Exists(new_id))
        {
            new_id = long.Parse(Utils.RandomStrFromChar("123456789", 1) + Utils.GenerateRandomDigit(8));
        }

        return new_id;
    }

    public new static async Task<Deck> Load(long id)
    {
        if (id != DUMMY_DECK_ID)
            return await BasePlayerData<Deck, long>.Load(id);

        return new Deck
        {
            id = DUMMY_DECK_ID,
            leaderUnitID = Unit.DUMMY_UNIT_ID,
            name = null,
            units = new List<long> {Unit.DUMMY_UNIT_ID}
        };
    }

    public override async Task Save()
    {
        if (id == DUMMY_DECK_ID)
            return;
        await base.Save();
    }

    public override long Identifier => id;
}