using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer;

public class ExchangeItemListEntity : BaseEntity<ExchangeItemListEntity>
{
    public ExchangeItemListEntity(
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
        
        //using var cardsDb = new CardsContext();
        Response boothResponse = InitExchangeItemResponse;
        
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

    public static readonly Response InitExchangeItemResponse = new()
    {
        exchange = new()
        {
            exchange_id = 100000000,
            start_at = 1483228800,
            end_at = 1893456000,
            special_chapter_id = null,
            products = new Dictionary<string, ExchangeProductData>
            {
                {  "1", NewCardExchangeProduct(id: 100000001, masterId: 500140, name: "結城友奈") },
                {  "2", NewCardExchangeProduct(id: 100000002, masterId: 500150, name: "東郷美森") },
                {  "3", NewCardExchangeProduct(id: 100000003, masterId: 500160, name: "犬吠埼風") },
                {  "4", NewCardExchangeProduct(id: 100000004, masterId: 500170, name: "犬吠埼樹") },
                {  "5", NewCardExchangeProduct(id: 100000005, masterId: 500180, name: "犬吠埼樹") },
                {  "6", NewCardExchangeProduct(id: 100000006, masterId: 500190, name: "乃木園子") },
                {  "7", NewCardExchangeProduct(id: 100000007, masterId: 500010, name: "源モモ") },
                {  "8", NewCardExchangeProduct(id: 100000008, masterId: 500020, name: "半蔵門雪") },
                {  "9", NewCardExchangeProduct(id: 100000009, masterId: 500030, name: "八千代命") },
                { "10", NewCardExchangeProduct(id: 100000010, masterId: 500040, name: "相模楓") },
                { "11", NewCardExchangeProduct(id: 100000011, masterId: 500050, name: "青葉初芽") },
                { "12", NewCardExchangeProduct(id: 100000012, masterId: 500060, name: "石川五恵") },
                { "13", NewCardExchangeProduct(id: 100000013, masterId: 500070, name: "衛藤可奈美") },
                { "14", NewCardExchangeProduct(id: 100000014, masterId: 500080, name: "十条姫和") },
                { "15", NewCardExchangeProduct(id: 100000015, masterId: 500090, name: "獅童真希") },
                { "16", NewCardExchangeProduct(id: 100000016, masterId: 520000, name: "衛藤可奈美") },
                { "17", NewCardExchangeProduct(id: 100000017, masterId: 530000, name: "御坂美琴") },
                { "18", NewCardExchangeProduct(id: 100000017, masterId: 530010, name: "白井黒子") },
                { "19", NewCardExchangeProduct(id: 100000019, masterId: 500100, name: "初春飾利") },
                { "20", NewCardExchangeProduct(id: 100000020, masterId: 500110, name: "佐天涙子") },
                { "21", NewCardExchangeProduct(id: 100000021, masterId: 500120, name: "御坂美琴") },
                { "22", NewCardExchangeProduct(id: 100000022, masterId: 500130, name: "白井黒子") }
            }
        }
    };


    private static ExchangeProductData NewCardExchangeProduct(long id, long masterId, string name)
    {
        return new()
        {
            id = id,
            master_id = masterId,
            from_content_id = 21,
            special_chapter_id = null,
            item_category = 1,
            purchased_quantity = 0,
            purchase_limit_quantity = 99,
            point = 10000,
            name = name,
            quantity = 1,
            have_story = false
        };
    }
}