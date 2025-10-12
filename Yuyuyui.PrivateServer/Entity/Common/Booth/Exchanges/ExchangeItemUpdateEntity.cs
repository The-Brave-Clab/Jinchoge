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
        RouteConfig config)
        : base(requestUri, httpMethod, requestHeaders, requestBody, config)
    {
    }

    protected override async Task ProcessRequest()
    {
        var playerId = await GetPlayerIdFromCookies();
        
        Request exchangeBoothRequest = Deserialize<Request>(requestBody)!;
        long exchangeItemId = exchangeBoothRequest.exchange_booth_item_id;

        ExchangeProductData? cardProduct =
            ExchangeItemListEntity.GetInitExchangeItemResponse().exchange.products.Values
                .Where(product => product.item_category == 1) // cards
                .FirstOrDefault(product => product.id == exchangeItemId);

        if (cardProduct == null)
        {
            SetBasicResponseHeaders();
            return;
        }

        long masterCardId = cardProduct.master_id;
        int potentialCount = exchangeBoothRequest.count;

        await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
        {
            var player = await PlayerProfile.Load(playerId.code);
            await player.GrantCard(masterCardId, potentialCount);
        }

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