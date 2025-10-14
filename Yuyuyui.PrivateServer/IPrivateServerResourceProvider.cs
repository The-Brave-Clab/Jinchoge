using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer;

public interface IPrivateServerResourceProvider
{
    IPlayerDataProvider<TPlayerData, TIdentifier> GetDataProvider<TPlayerData, TIdentifier>()
        where TPlayerData : BasePlayerData<TPlayerData, TIdentifier>
        where TIdentifier : notnull;

    IPlayerProfileSessionProvider sessionProvider { get; }
    IMasterDataProvider masterDataProvider { get; }
    IInGameConfigProvider inGameConfigProvider { get; }
    IDistributedLockProvider distributedLockProvider { get; }
    IInGameURLProvider urlProvider { get; }

    Uri RewriteRequestUri(Uri originalRequestUri);

    void OverrideRouteConfigs(Dictionary<Type, RouteConfig> configs);
}