namespace Yuyuyui.PrivateServer;

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
        var player = GetPlayerFromCookies();

        responseBody = Serialize(new BoothResponse { exchange = new() });
        SetBasicResponseHeaders();

        return Task.CompletedTask;
    }
}