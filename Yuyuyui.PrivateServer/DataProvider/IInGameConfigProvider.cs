using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer;

public interface IInGameConfigProvider
{
    public static IInGameConfigProvider? ActiveProvider { get; set; } = null;

    public static readonly List<string> SupportedInGameScenarioLanguage = ["ja", "zh", "en"];

    Task<string> GetScenarioLanguage(PlayerProfile playerProfile);
    Task SetScenarioLanguage(PlayerProfile playerProfile, string language);

    Task<bool> GetInfiniteItems(PlayerProfile playerProfile);
    Task SetInfiniteItems(PlayerProfile playerProfile, bool value);

    Task<bool> GetUnlockAllDifficulties(PlayerProfile playerProfile);
    Task SetUnlockAllDifficulties(PlayerProfile playerProfile, bool value);
}