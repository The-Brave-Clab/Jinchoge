using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer.AWS;

// TODO: This is a stub implementation. Implement it properly later.
public class ConfigPlayerInGameConfigProvider : IInGameConfigProvider
{
    public Task<string> GetScenarioLanguage(PlayerProfile playerProfile)
    {
        return Task.FromResult(IInGameConfigProvider.SupportedInGameScenarioLanguage[0]);
    }

    public Task SetScenarioLanguage(PlayerProfile playerProfile, string language)
    {
        return Task.CompletedTask;
    }

    public Task<bool> GetInfiniteItems(PlayerProfile playerProfile)
    {
        return Task.FromResult(true);
    }

    public Task SetInfiniteItems(PlayerProfile playerProfile, bool value)
    {
        return Task.CompletedTask;
    }

    public Task<bool> GetUnlockAllDifficulties(PlayerProfile playerProfile)
    {
        return Task.FromResult(true);
    }

    public Task SetUnlockAllDifficulties(PlayerProfile playerProfile, bool value)
    {
        return Task.CompletedTask;
    }
}