using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer;

public class TradeBoothsEntity : BaseEntity<TradeBoothsEntity>
{
    public TradeBoothsEntity(
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
        // var player = GetPlayerFromCookies();

        responseBody = Serialize(new ExchangeItemListEntity.Response());
        SetBasicResponseHeaders();

        return Task.CompletedTask;
    }
}