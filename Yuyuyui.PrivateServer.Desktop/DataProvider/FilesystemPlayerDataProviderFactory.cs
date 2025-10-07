namespace Yuyuyui.PrivateServer.Desktop;

public class FilesystemPlayerDataProviderFactory : PlayerDataProviderFactory
{
    protected override IPlayerDataProvider<TPlayerData, TIdentifier> Create<TPlayerData, TIdentifier>()
    {
        return new FilesystemPlayerDataProvider<TPlayerData, TIdentifier>();
    }
}