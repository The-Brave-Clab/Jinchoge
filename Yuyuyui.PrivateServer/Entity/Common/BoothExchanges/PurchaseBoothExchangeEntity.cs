using Yuyuyui.PrivateServer.Constants;
using Yuyuyui.PrivateServer.DataModel;
using Yuyuyui.PrivateServer.Requests;
using Yuyuyui.PrivateServer.Responses.Booth;
using Yuyuyui.PrivateServer.Responses.BoothExchanges;
using Yuyuyui.PrivateServer.Responses.BoothExchanges.Exchange;
using Yuyuyui.PrivateServer.Responses.BoothExchanges.Product;
using Yuyuyui.PrivateServer.Strategy;

namespace Yuyuyui.PrivateServer.Entity.Common.BoothExchanges;

public class PurchaseBoothExchangeEntity : BaseEntity<PurchaseBoothExchangeEntity>
{
    private static int CARD_CATEGORY = 1;

    public PurchaseBoothExchangeEntity(
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
        
        ExchangeBoothRequest exchangeBoothRequest = Deserialize<ExchangeBoothRequest>(requestBody);
        long exchangeItemId = exchangeBoothRequest.exchange_booth_item_id;

        BoothExchangeProduct cardProduct = FindCardProduct(exchangeItemId);

        if (cardProduct == null)
        {
            SetBasicResponseHeaders();
            return Task.CompletedTask;
        }

        using var cardsDb = new CardsContext();
        using var itemsDb = new ItemsContext();
        using var giftsDb = new GiftsContext();

        long masterCardId = cardProduct.master_id;
        int potentialCount = exchangeBoothRequest.count;
        UpsertCardToProfileStrategy.Handle(player, masterCardId, potentialCount, cardsDb, giftsDb, itemsDb);

        ExchangeResponse currentResponse = new ExchangeResponse(
            exchange: new PurchaseExchange(product: new ExchangeProduct(
                id: exchangeItemId,
                purchasedQuantity: exchangeBoothRequest.count
            ))
        );

        responseBody = Serialize(currentResponse);
        
        SetBasicResponseHeaders();
        return Task.CompletedTask;
    }
    
    private static BoothExchangeProduct FindCardProduct(long exchangeItemId)
    {
        return BoothConstants.InitExchangeItemResponse()
            .exchange
            .products
            .Values
            .Where(product => product.item_category == CARD_CATEGORY)
            .Single(product => product.id == exchangeItemId);
    }
}