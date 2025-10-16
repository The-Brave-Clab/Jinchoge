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

    public string APIBase => "https://app.yuyuyui.jp";
    public string Regulation => $"http://{PRIVATE_LOCAL_API_SERVER}";

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

    public IList<ArticleEntity.Response.Article> GetArticles()
    {
        return
        [
            new()
            {
                url = $"http://{PRIVATE_LOCAL_API_SERVER}/{ServerResources.RELEASE_NOTES_PATH}",
                kind = 0,
                label = "topics"
            },
            new()
            {
                url = $"http://{PRIVATE_LOCAL_API_SERVER}",
                kind = 0,
                label = "defect_topics"
            },
            new()
            {
                url = $"http://{PRIVATE_LOCAL_API_SERVER}",
                kind = 0,
                label = "inquiry"
            },
            new()
            {
                url = $"http://{PRIVATE_LOCAL_API_SERVER}",
                kind = 0,
                label = "terms"
            },
            new()
            {
                url = $"http://{PRIVATE_LOCAL_API_SERVER}",
                kind = 0,
                label = "helps"
            },
            new()
            {
                url = $"http://{PRIVATE_LOCAL_API_SERVER}",
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
                transition_url = $"http://{PRIVATE_LOCAL_API_SERVER}",
                available_user_level = 0
            },
        ];
    }
}