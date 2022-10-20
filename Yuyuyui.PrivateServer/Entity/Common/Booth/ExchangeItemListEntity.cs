using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer;

public class ExchangeItemListEntity : BaseEntity<ExchangeItemListEntity>
{
    public ExchangeItemListEntity(
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

        Response boothResponse;
        using (var cardsDb = new CardsContext())
            boothResponse = GetInitExchangeItemResponse(cardsDb);
        
        // boothResponse.exchange.products.Values
        //     .Where(product => product.item_category == 1)
        //     .Where(product => player.cards.ContainsKey(product.master_id))
        //     .ForEach(product => product.purchased_quantity = 88);
        
        responseBody = Serialize(boothResponse);
        SetBasicResponseHeaders();

        return Task.CompletedTask;
    }
    
    public class Response
    {
        public Result exchange { get; set; } = new();

        public class Result
        {
            public long exchange_id { get; set; }
            public long start_at { get; set; } // unix time
            public long end_at { get; set; } // unix time
            public long? special_chapter_id { get; set; } = null;
            public IDictionary<string, ExchangeProductData> products = new Dictionary<string, ExchangeProductData>();
        }
    }

    public static Response GetInitExchangeItemResponse(CardsContext cardsDb)
    {
        return new()
        {
            exchange = new()
            {
                exchange_id = 100000000,
                start_at = 1483228800,
                end_at = 1893456000,
                special_chapter_id = null,
                products = cardsDb.Cards.ToList()
                    .Where(c => c.Id == c.BaseCardId)
                    .Where(c => c.BaseCardId != 700010) // remove Akamine dummy
                    .Where(c => c.BaseCardId != 800100) // remove Washio test
                    .ToDictionary(
                        c => $"{c.BaseCardId}",
                        c => NewCardExchangeProduct(c.BaseCardId + 100000000, c.BaseCardId, c.Name)
                    )
            }
        };
    }


    private static ExchangeProductData NewCardExchangeProduct(long id, long masterId, string name)
    {
        return new()
        {
            id = id,
            master_id = masterId,
            from_content_id = 21,
            special_chapter_id = null,
            item_category = 1, // card
            purchased_quantity = 0,
            purchase_limit_quantity = 99,
            point = 1,
            name = name,
            quantity = 1,
            have_story = false
        };
    }
}