using Yuyuyui.PrivateServer.Constants;

namespace Yuyuyui.PrivateServer.Entity.Common.Booth;

public class EventBoothItemListEntity : BaseEntity<EventBoothItemListEntity>
{
    public EventBoothItemListEntity(
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
        
        responseBody = Serialize(BoothConstants.EventItemResponse);
        SetBasicResponseHeaders();

        return Task.CompletedTask;
    }
}