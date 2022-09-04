using Yuyuyui.PrivateServer.Constants;

namespace Yuyuyui.PrivateServer.Entity.Common.Booth;

public class ExchangeBoothItemListEntity : BaseEntity<ExchangeBoothItemListEntity>
{
    public ExchangeBoothItemListEntity(
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
        responseBody = Serialize(BoothConstants.ExchangeItemResponse);
        SetBasicResponseHeaders();

        return Task.CompletedTask;
    }
}