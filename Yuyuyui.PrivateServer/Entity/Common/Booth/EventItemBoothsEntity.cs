using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer;

public class EventItemBoothsEntity : BaseEntity<EventItemBoothsEntity>
{
    public EventItemBoothsEntity(
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
        //var player = GetPlayerFromCookies();

        ExchangeItemListEntity.Response boothResponse;
        using (var cardsDb = new CardsContext())
            boothResponse = ExchangeItemListEntity.GetInitExchangeItemResponse(cardsDb);

        responseBody = Serialize(boothResponse);
        SetBasicResponseHeaders();

        return Task.CompletedTask;
    }
}