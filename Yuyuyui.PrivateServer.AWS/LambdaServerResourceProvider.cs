using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.AWS;

public class LambdaServerResourceProvider : IPrivateServerResourceProvider
{
    private DynamoDBPlayerDataProviderFactory playerDataProviderFactory = new();
    private DynamoDBPlayerProfileSessionProvider lambdaSessionProvider = new();
    private LambdaMasterDataProvider lambdaMasterDataProvider = new();
    private ConfigPlayerInGameConfigProvider lambdaInGameConfigProvider = new();
    private DynamoDBLockProvider lambdaDistributedLockProvider = new();

    public IPlayerDataProvider<TPlayerData, TIdentifier> GetDataProvider<TPlayerData, TIdentifier>() where TPlayerData : BasePlayerData<TPlayerData, TIdentifier> where TIdentifier : notnull
    {
        return playerDataProviderFactory.Get<TPlayerData, TIdentifier>();
    }

    public IPlayerProfileSessionProvider sessionProvider => lambdaSessionProvider;
    public IMasterDataProvider masterDataProvider => lambdaMasterDataProvider;
    public IInGameConfigProvider inGameConfigProvider => lambdaInGameConfigProvider;
    public IDistributedLockProvider distributedLockProvider => lambdaDistributedLockProvider;

    public string APIBase => "https://app.yuyuyui.jp";
    public string Regulation => "https://app.yuyuyui.jp/api/v1/placeholder/regulation";

    public Uri RewriteRequestUri(Uri originalRequestUri)
    {
        return originalRequestUri;
    }

    public void OverrideRouteConfigs(Dictionary<Type, RouteConfig> configs)
    {
        configs[typeof(PlaceholderEntity)] = new RouteConfig("/placeholder/{param}", "GET");
    }

    public IList<ArticleEntity.Response.Article> GetArticles()
    {
        return
        [
            new()
            {
                url = "https://app.yuyuyui.jp/api/v1/placeholder/topics",
                kind = 0,
                label = "topics"
            },
            new()
            {
                url = "https://app.yuyuyui.jp/api/v1/placeholder/defects",
                kind = 0,
                label = "defect_topics"
            },
            new()
            {
                url = "https://app.yuyuyui.jp/api/v1/placeholder/inquiry",
                kind = 0,
                label = "inquiry"
            },
            new()
            {
                url = "https://app.yuyuyui.jp/api/v1/placeholder/terms",
                kind = 0,
                label = "terms"
            },
            new()
            {
                url = "https://app.yuyuyui.jp/api/v1/placeholder/helps",
                kind = 0,
                label = "helps"
            },
            new()
            {
                url = "https://app.yuyuyui.jp",
                kind = 0,
                label = "official_links"
            }
        ];
    }

    public IList<Banner> GetBanners()
    {
        return
        [
            // new()
            // {
            //     image_id = 37930,
            //     transition_screen_kind = "gacha/limited",
            //     transition_url = "",
            //     available_user_level = 0
            // },
            // new()
            // {
            //     image_id = 1529,
            //     transition_screen_kind = "topics",
            //     transition_url = "4522",
            //     available_user_level = 0
            // },
            // new()
            // {
            //     image_id = 1441,
            //     transition_screen_kind = "story",
            //     transition_url = "",
            //     available_user_level = 0
            // },
            // new()
            // {
            //     image_id = 71970,
            //     transition_screen_kind = "shop/package",
            //     transition_url = "",
            //     available_user_level = 0
            // },
            // new()
            // {
            //     image_id = 90020,
            //     transition_screen_kind = "",
            //     transition_url = "https://yuyuyu.tv/churutto/",
            //     available_user_level = 0
            // },
            new()
            {
                image_id = 9000,
                transition_screen_kind = "",
                transition_url = "https://app.yuyuyui.jp",
                available_user_level = 0
            },
        ];
    }
}