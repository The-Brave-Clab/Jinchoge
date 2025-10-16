using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer;

public class BannerEntity : BaseEntity<BannerEntity>
{
    public BannerEntity(
        Uri requestUri,
        string httpMethod,
        Dictionary<string, string> requestHeaders,
        byte[] requestBody,
        RouteConfig config)
        : base(requestUri, httpMethod, requestHeaders, requestBody, config)
    {
    }

    protected override Task ProcessRequest()
    {
        // Utils.LogWarning("Stub API!");
            
        Response responseObj = new()
        {
            banners = PrivateServer.ResourceProvider.GetBanners()
        };

        responseBody = Serialize(responseObj);
        SetBasicResponseHeaders();

        return Task.CompletedTask;
    }

    public class Response
    {
        public IList<Banner> banners { get; set; } = new List<Banner>();
    }
}