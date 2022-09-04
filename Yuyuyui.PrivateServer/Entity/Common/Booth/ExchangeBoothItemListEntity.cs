using Yuyuyui.PrivateServer.Constants;
using Yuyuyui.PrivateServer.DataModel;
using Yuyuyui.PrivateServer.Responses;

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
        var player = GetPlayerFromCookies();
        
        using var cardsDb = new CardsContext();
        BoothResponse boothResponse = BoothConstants.InitExchangeItemResponse();
        
        // boothResponse.exchange.products.Values
        //     .Where(product => product.item_category == 1)
        //     .Where(product => player.cards.ContainsKey(product.master_id))
        //     .ForEach(product => product.purchased_quantity = 88);
        
        responseBody = Serialize(boothResponse);
        SetBasicResponseHeaders();

        return Task.CompletedTask;
    }
}