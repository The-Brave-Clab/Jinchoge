using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Yuyuyui.PrivateServer.Desktop;

public class DesktopServerResourceProvider : IPrivateServerResourceProvider
{
    public static readonly HttpClient HttpClient = new();

    public const string PRIVATE_LOCAL_API_SERVER = "private.yuyuyui.org";

    private FilesystemPlayerDataProviderFactory playerDataProviderFactory = new();
    private InMemoryPlayerProfileSessionProvider desktopSessionProvider = new();
    private AWSMasterDataProvider desktopMasterDataProvider = new();
    private DesktopInGameConfigProvider desktopInGameConfigProvider = new();
    private LocalLockProvider desktopDistributedLockProvider = new();
    private DesktopInGameURLProvider desktopInGameURLProvider = new();

    public AWSMasterDataProvider AWSMasterDataProvider => desktopMasterDataProvider;

    static DesktopServerResourceProvider()
    {
        HttpClient.DefaultRequestHeaders.Referrer = new Uri($"https://{PRIVATE_LOCAL_API_SERVER}");
    }

    public IPlayerDataProvider<TPlayerData, TIdentifier> GetDataProvider<TPlayerData, TIdentifier>()
        where TPlayerData : BasePlayerData<TPlayerData, TIdentifier>
        where TIdentifier : notnull
    {
        return playerDataProviderFactory.Get<TPlayerData, TIdentifier>();
    }

    public IPlayerProfileSessionProvider sessionProvider => desktopSessionProvider;
    public IMasterDataProvider masterDataProvider => desktopMasterDataProvider;
    public IInGameConfigProvider inGameConfigProvider => desktopInGameConfigProvider;
    public IDistributedLockProvider distributedLockProvider => desktopDistributedLockProvider;
    public IInGameURLProvider urlProvider => desktopInGameURLProvider;

    public Uri RewriteRequestUri(Uri originalRequestUri)
    {
        // replace the host with the private server host
        if (!originalRequestUri.Host.Equals(PrivateServer.OFFICIAL_API_SERVER, StringComparison.OrdinalIgnoreCase))
            return originalRequestUri;

        UriBuilder builder = new(originalRequestUri)
        {
            Host = PRIVATE_LOCAL_API_SERVER
        };
        return builder.Uri;
    }

    public void OverrideRouteConfigs(Dictionary<Type, RouteConfig> configs)
    {
        // Does nothing
    }
}