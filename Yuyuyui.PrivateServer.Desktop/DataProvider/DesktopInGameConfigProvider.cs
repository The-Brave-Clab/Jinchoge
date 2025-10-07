using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer.Desktop;

public class DesktopInGameConfigProvider : IInGameConfigProvider
{
    private readonly Config.ConfigObject config = Config.Get();

    public Task<string> GetScenarioLanguage(PlayerProfile playerProfile)
    {
        return Task.FromResult(config.InGame.ScenarioLanguage);
    }

    public Task SetScenarioLanguage(PlayerProfile playerProfile, string language)
    {
        config.InGame.ScenarioLanguage = language;
        return Task.CompletedTask;
    }

    public Task<bool> GetInfiniteItems(PlayerProfile playerProfile)
    {
        return Task.FromResult(config.InGame.InfiniteItems);
    }

    public Task SetInfiniteItems(PlayerProfile playerProfile, bool value)
    {
        config.InGame.InfiniteItems = value;
        return Task.CompletedTask;
    }

    public Task<bool> GetUnlockAllDifficulties(PlayerProfile playerProfile)
    {
        return Task.FromResult(config.InGame.UnlockAllDifficulties);
    }

    public Task SetUnlockAllDifficulties(PlayerProfile playerProfile, bool value)
    {
        config.InGame.UnlockAllDifficulties = value;
        return Task.CompletedTask;
    }
}