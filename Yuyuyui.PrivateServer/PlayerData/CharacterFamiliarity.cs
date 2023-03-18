using System.Collections.Generic;
using System.Linq;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer;

// Used in json
public class CharacterFamiliarity
{
    public string character_group { get; set; } = "";
    public int rank { get; set; }
    public int familiarity { get; set; }

    public static string GetGroupName(long characterId1, long characterId2)
    {
        if (characterId1 < characterId2)
            return $"{characterId1}-{characterId2}";
        return $"{characterId2}-{characterId1}";
    }

    private static readonly Dictionary<long, int> enhancementAmount = new()
    {
        {1, 1},
        {2, 2},
        {3, 5},
        {4, 2},
        {5, 5},
        {6, 10},
        {7, 10},
        {8, 5},
        {9, 10},
        {1000, 2} // character specific udon
    };

    public static int GetEnhancement(long itemId)
    {
        if (itemId < 1000) return enhancementAmount[itemId];
        return enhancementAmount[1000];
    }

    public FamiliarityLevel GetLevelData(CharactersContext characterDb)
    {
        return characterDb.FamiliarityLevels.First(l => l.Level == rank);
    }

    public CharacterFamiliarityChange UpdateAndGetChange(CharactersContext charactersDb, int gotFamiliarity)
    {
        var maxLevel = charactersDb.FamiliarityLevels.OrderBy(l => l.Level).Last();
        var maxExp = charactersDb.FamiliarityLevels.First(l => l.Level == maxLevel.Level - 1).MaxExp!.Value + 1;
        CharacterFamiliarityChange change = new();
        change.character_group = character_group;
        change.before_familiarity = familiarity;
        change.before_rank = rank;

        var newFamiliarityUncapped = change.before_familiarity + gotFamiliarity;
        FamiliarityLevel newRankUncapped = CalcUtil.GetFamiliarityRankFromExp(charactersDb, newFamiliarityUncapped);
        bool familiarityOverflow = newRankUncapped.Level >= maxLevel.Level;
        int newRank = familiarityOverflow ? maxLevel.Level : newRankUncapped.Level;
        int newFamiliarity = familiarityOverflow
            ? maxExp
            : newFamiliarityUncapped;

        rank = newRank;
        familiarity = newFamiliarity;
        change.rank = rank;
        change.familiarity = familiarity;

        return change;
    }
}

// Used in json
public class CharacterFamiliarityChange : CharacterFamiliarity
{
    public int before_familiarity { get; set; }
    public int before_rank { get; set; }
}

// Used in json
public class CharacterFamiliarityWithAssist : CharacterFamiliarity
{
    public int assist_level { get; set; }

    public CharacterFamiliarityChangeWithAssist UpdateAndGetChange(CharactersContext charactersDb,
        int gotFamiliarity, int gotAssistLevel)
    {
        var beforeAssistLevel = assist_level;
        CharacterFamiliarityChange change = UpdateAndGetChange(charactersDb, gotFamiliarity);
        CharacterFamiliarityChangeWithAssist changeWithAssist = new()
        {
            character_group = change.character_group,
            rank = change.rank,
            familiarity = change.familiarity,
            before_rank = change.before_rank,
            before_familiarity = change.before_familiarity,
            before_assist_level = beforeAssistLevel
        };
        
        int newAssistLevel = beforeAssistLevel + gotAssistLevel; // will this overflow?
        assist_level = newAssistLevel;
        changeWithAssist.assist_level = assist_level;

        return changeWithAssist;
    }
}

// Used in json
public class CharacterFamiliarityChangeWithAssist : CharacterFamiliarityChange
{
    public int assist_level { get; set; }
    public int before_assist_level { get; set; }
}