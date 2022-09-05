﻿namespace Yuyuyui.PrivateServer;

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
        //var player = GetPlayerFromCookies();
        
        responseBody = Serialize(new BoothEventResponse());
        SetBasicResponseHeaders();

        return Task.CompletedTask;
    }
}