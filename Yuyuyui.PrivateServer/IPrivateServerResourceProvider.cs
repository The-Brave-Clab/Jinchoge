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

    string APIBase { get; }
    string Regulation { get; }

    Uri RewriteRequestUri(Uri originalRequestUri);

    void OverrideRouteConfigs(Dictionary<Type, RouteConfig> configs);

    IList<ArticleEntity.Response.Article> GetArticles();

    IList<Banner> GetBanners();
}