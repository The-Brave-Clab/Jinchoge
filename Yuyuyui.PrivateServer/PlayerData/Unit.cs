using System.Collections.Generic;
using System.Linq;
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

    public static long GetID()
    {
        long new_id = long.Parse(Utils.RandomStrFromChar("123456789", 1) + Utils.GenerateRandomDigit(8));
        while (Exists(new_id))
        {
            new_id = long.Parse(Utils.RandomStrFromChar("123456789", 1) + Utils.GenerateRandomDigit(8));
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

    public int GetHP(CardsContext cardsDb, CharactersContext charactersDb, PlayerProfile belongTo)
    {
        // TODO: not finished yet!
        if (baseCardID == null) return 0;
        int hp = 0;
        var card = GetCard()!;
        var masterCard = card.MasterData(cardsDb);
        hp += card.GetHitPoint(cardsDb);

        if (supportCardID != null)
        {
            var supportCard = Support()!;
            var masterSupport = supportCard.MasterData(cardsDb);
            hp += supportCard.GetHitPoint(cardsDb);
            
            // get bonus
            CharacterFamiliarity familiarity =
                belongTo.GetCharacterFamiliarity(masterCard.CharacterId, masterSupport.CharacterId);
            int bonus = CalcUtil.AssistLevelHitPointBonus(familiarity.assist_level);
            float coefficient = familiarity.GetLevelData(charactersDb).HitPointCoefficient;

            hp = CalcUtil.CalcAddedFamiliarityAndAssist(hp, bonus, coefficient);
        }

        if (supportCard2ID != null)
            hp += Support2()!.GetHitPoint(cardsDb);

        if (assistCardID != null)
            hp += Assist()!.GetHitPoint(cardsDb);

        return hp;
    }

    public int GetAtk(CardsContext cardsDb, CharactersContext charactersContext, PlayerProfile belongTo)
    {
        // TODO: not finished yet!
        if (baseCardID == null) return 0;
        int atk = 0;
        var card = GetCard()!;
        var masterCard = card.MasterData(cardsDb);
        atk += card.GetAttack(cardsDb);

        if (supportCardID != null)
        {
            var supportCard = Support()!;
            var masterSupport = supportCard.MasterData(cardsDb);
            atk += supportCard.GetAttack(cardsDb);
            
            // get bonus
            CharacterFamiliarity familiarity =
                belongTo.GetCharacterFamiliarity(masterCard.CharacterId, masterSupport.CharacterId);
            int bonus = CalcUtil.AssistLevelAttackBonus(familiarity.assist_level);
            float coefficient = familiarity.GetLevelData(charactersContext).AttackCoefficient;

            atk = CalcUtil.CalcAddedFamiliarityAndAssist(atk, bonus, coefficient);
        }

        if (supportCard2ID != null)
            atk += Support2()!.GetAttack(cardsDb);

        if (assistCardID != null)
            atk += Assist()!.GetAttack(cardsDb);

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

        public static CardWithSupport? FromUnit(CardsContext cardsDb, CharactersContext charactersDb, 
            Unit? unit, PlayerProfile? belongTo)
        {
            if (unit == null) return null;
            return new CardWithSupport
            {
                id = unit.id,
                hit_point = unit.GetHP(cardsDb, charactersDb, belongTo!),
                attack = unit.GetAtk(cardsDb, charactersDb, belongTo!),
                user_card_id = unit.baseCardID,
                support = unit.Support()?.AsSupport().ToDict(cardsDb) ?? new Dictionary<string, long>(),
                support_2 = unit.Support2()?.AsSupport().ToDict(cardsDb) ?? new Dictionary<string, long>(),
                assist = unit.Assist()?.AsSupport().ToDict(cardsDb) ?? new Dictionary<string, long>(),
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