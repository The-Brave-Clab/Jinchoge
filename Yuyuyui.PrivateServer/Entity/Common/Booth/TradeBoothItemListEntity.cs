using Yuyuyui.PrivateServer.Constants;

namespace Yuyuyui.PrivateServer.Entity.Common.Booth;

public class TradeBoothItemListEntity : BaseEntity<TradeBoothItemListEntity>
{
    public TradeBoothItemListEntity(
        Uri requestUri,
        string httpMethod,
        Dictionary<string, string> requestHeaders,
        byte[] requestBody,
        Config config)
        : base(requestUri, httpMethod, requestHeaders, requestBody, config)
    {
    }

    protected override Task ProcessRequest()
    {
        responseBody = Serialize(BoothConstants.TradeItemResponse);
        SetBasicResponseHeaders();

        return Task.CompletedTask;
    }
}