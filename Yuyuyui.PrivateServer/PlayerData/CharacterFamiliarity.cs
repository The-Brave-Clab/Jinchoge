using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer;

// Used in json
public class CharacterFamiliarity
{
    public string character_group { get; set; } = "";
    public int rank { get; set; }
    public int familiarity { get; set; }
    public int assist_level { get; set; }

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
}

// Used in json
public class CharacterFamiliarityChange : CharacterFamiliarity
{
    public int before_familiarity { get; set; }
    public int before_rank { get; set; }
    public int before_assist_level { get; set; }
}