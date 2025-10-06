using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

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

    public static async Task<long> GetID()
    {
        long new_id = long.Parse(Utils.RandomStrFromChar("123456789", 1) + Utils.GenerateRandomDigit(8));
        while (await Exists(new_id))
        {
            new_id = long.Parse(Utils.RandomStrFromChar("123456789", 1) + Utils.GenerateRandomDigit(8));
        }

        return new_id;
    }

    public async Task<Card?> GetCard()
    {
        return baseCardID == null ? null : await Card.Load((long) baseCardID);
    }

    public static async Task<Unit> CreateEmptyUnit()
    {
        return new()
        {
            id = await GetID(),
            baseCardID = null,
            supportCardID = null,
            supportCard2ID = null,
            assistCardID = null,
            accessories = new List<long>(),
        };
    }

    public async Task<int> GetHP(PlayerProfile belongTo)
    {
        // TODO: not finished yet!
        if (baseCardID == null) return 0;
        int hp = 0;
        var card = (await GetCard())!;
        var masterCard = card.MasterData();
        hp += card.GetHitPoint();

        if (supportCardID != null)
        {
            var supportCard = (await Support())!;
            var masterSupport = supportCard.MasterData();
            hp += supportCard.GetHitPoint();
            
            // get bonus
            CharacterFamiliarityWithAssist familiarity =
                await belongTo.GetCharacterFamiliarity(masterCard.CharacterId, masterSupport.CharacterId);
            int bonus = CalcUtil.AssistLevelHitPointBonus(familiarity.assist_level);
            float coefficient = familiarity.GetLevelData().HitPointCoefficient;

            hp = CalcUtil.CalcAddedFamiliarityAndAssist(hp, bonus, coefficient);
        }

        if (supportCard2ID != null)
            hp += (await Support2())!.GetHitPoint();

        if (assistCardID != null)
            hp += (await Assist())!.GetHitPoint();

        return hp;
    }

    public async Task<int> GetAtk(PlayerProfile belongTo)
    {
        // TODO: not finished yet!
        if (baseCardID == null) return 0;
        int atk = 0;
        var card = (await GetCard())!;
        var masterCard = card.MasterData();
        atk += card.GetAttack();

        if (supportCardID != null)
        {
            var supportCard = (await Support())!;
            var masterSupport = supportCard.MasterData();
            atk += supportCard.GetAttack();
            
            // get bonus
            CharacterFamiliarityWithAssist familiarity =
                await belongTo.GetCharacterFamiliarity(masterCard.CharacterId, masterSupport.CharacterId);
            int bonus = CalcUtil.AssistLevelAttackBonus(familiarity.assist_level);
            float coefficient = familiarity.GetLevelData().AttackCoefficient;

            atk = CalcUtil.CalcAddedFamiliarityAndAssist(atk, bonus, coefficient);
        }

        if (supportCard2ID != null)
            atk += (await Support2())!.GetAttack();

        if (assistCardID != null)
            atk += (await Assist())!.GetAttack();

        return atk;
    }

    public async Task<long?> GetMasterId()
    {
        if (baseCardID == null) return null;
        var baseCard = await Card.Load((long)baseCardID);
        return baseCard.master_id;
    }

    public async Task<int?> GetPotential()
    {
        if (baseCardID == null) return null;
        var baseCard = await Card.Load((long)baseCardID);
        return baseCard.potential;
    }

    public async Task<int?> GetEvolutionLevel()
    {
        if (baseCardID == null) return null;
        var baseCard = await Card.Load((long)baseCardID);
        return baseCard.evolution_level;
    }

    public async Task<int?> GetLevel()
    {
        if (baseCardID == null) return null;
        var baseCard = await Card.Load((long)baseCardID);
        return baseCard.level;
    }

    public async Task<Card?> Support()
    {
        return supportCardID == null ? null : await Card.Load((long) supportCardID);
    }

    public async Task<Card?> Support2()
    {
        return supportCard2ID == null ? null : await Card.Load((long) supportCard2ID);
    }

    public async Task<Card?> Assist()
    {
        return assistCardID == null ? null : await Card.Load((long) assistCardID);
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

        public static async Task<CardWithSupport?> FromUnit(Unit? unit, PlayerProfile? belongTo)
        {
            if (unit == null) return null;
            var supportCard = (await unit.Support())?.AsSupport();
            var support2Card = (await unit.Support2())?.AsSupport();
            var assistCard = (await unit.Assist())?.AsSupport();
            var supportDict = supportCard != null ? await supportCard.ToDict() : new Dictionary<string, long>();
            var support2Dict = support2Card != null ? await support2Card.ToDict() : new Dictionary<string, long>();
            var assistDict = assistCard != null ? await assistCard.ToDict() : new Dictionary<string, long>();
            return new CardWithSupport
            {
                id = unit.id,
                hit_point = await unit.GetHP(belongTo!),
                attack = await unit.GetAtk(belongTo!),
                user_card_id = unit.baseCardID,
                support = supportDict,
                support_2 = support2Dict,
                assist = assistDict,
                accessories = (await unit.accessories.Select(Accessory.Load).WhenAll()).ToList(),
                master_id = await unit.GetMasterId(),
                potential = await unit.GetPotential(),
                evolution_level = await unit.GetEvolutionLevel(),
                level = await unit.GetLevel()
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