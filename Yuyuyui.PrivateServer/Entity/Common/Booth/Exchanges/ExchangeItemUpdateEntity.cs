using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer;

public class ExchangeItemUpdateEntity : BaseEntity<ExchangeItemUpdateEntity>
{
    public ExchangeItemUpdateEntity(
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
        
        Request exchangeBoothRequest = Deserialize<Request>(requestBody)!;
        long exchangeItemId = exchangeBoothRequest.exchange_booth_item_id;

        ExchangeProductData? cardProduct =
            ExchangeItemListEntity.InitExchangeItemResponse.exchange.products.Values
                .Where(product => product.item_category == 1) // cards
                .FirstOrDefault(product => product.id == exchangeItemId);

        if (cardProduct == null)
        {
            SetBasicResponseHeaders();
            return Task.CompletedTask;
        }

        long masterCardId = cardProduct.master_id;
        int potentialCount = exchangeBoothRequest.count;
        
        using (var cardsDb = new CardsContext())
        using (var itemsDb = new ItemsContext())
            player.GrantCard(masterCardId, potentialCount, cardsDb, itemsDb);

        Response currentResponse = new Response
        {
            exchange = new()
            {
                product = new()
                {
                    id = exchangeItemId,
                    purchased_quantity = exchangeBoothRequest.count
                }
            }
        };

        responseBody = Serialize(currentResponse);
        
        SetBasicResponseHeaders();
        return Task.CompletedTask;
    }

    public class Request
    {
        public long exchange_booth_id { get; set; }
        public long exchange_booth_item_id { get; set; }
        public int count { get; set; }
        public int before_count { get; set; }
    }

    public class Response
    {
        public Result exchange { get; set; } = new();

        public class Result
        {
            public ProductData product { get; set; } = new();

            public class ProductData
            {
                public long id { get; set; }
                public int purchased_quantity { get; set; }
            }
        }
    }
}