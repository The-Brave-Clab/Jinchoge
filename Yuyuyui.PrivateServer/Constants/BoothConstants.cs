﻿using Yuyuyui.PrivateServer.Responses;
using Yuyuyui.PrivateServer.Responses.Booth;

namespace Yuyuyui.PrivateServer.Constants;

public static class BoothConstants
{
    public static BoothResponse InitExchangeItemResponse()
    {
        return new BoothResponse(
            exchange: new BoothExchange(
                exchangeId: 100000000,
                startAt: 1483228800,
                endAt: 1893456000,
                products: new Dictionary<string, BoothExchangeProduct>()
                {
                    {
                        "1", NewCardExchangeProduct(id: 100000001, masterId: 500140, name: "結城友奈")
                    },
                    {
                        "2", NewCardExchangeProduct(id: 100000002, masterId: 500150, name: "東郷美森")
                    },
                    {
                        "3", NewCardExchangeProduct(id: 100000003, masterId: 500160, name: "犬吠埼風")
                    },
                    {
                        "4", NewCardExchangeProduct(id: 100000004, masterId: 500170, name: "犬吠埼樹")
                    },
                    {
                        "5", NewCardExchangeProduct(id: 100000005, masterId: 500180, name: "犬吠埼樹")
                    },
                    {
                        "6", NewCardExchangeProduct(id: 100000006, masterId: 500190, name: "乃木園子")
                    },
                    {
                        "7", NewCardExchangeProduct(id: 100000007, masterId: 500010, name: "源モモ")
                    },
                    {
                        "8", NewCardExchangeProduct(id: 100000008, masterId: 500020, name: "半蔵門雪")
                    },
                    {
                        "9", NewCardExchangeProduct(id: 100000009, masterId: 500030, name: "八千代命")
                    },
                    {
                        "10", NewCardExchangeProduct(id: 100000010, masterId: 500040, name: "相模楓")
                    },
                    {
                        "11", NewCardExchangeProduct(id: 100000011, masterId: 500050, name: "青葉初芽")
                    },
                    {
                        "12", NewCardExchangeProduct(id: 100000012, masterId: 500060, name: "石川五恵")
                    },
                    {
                        "13", NewCardExchangeProduct(id: 100000013, masterId: 500070, name: "衛藤可奈美")
                    },
                    {
                        "14", NewCardExchangeProduct(id: 100000014, masterId: 500080, name: "十条姫和")
                    },
                    {
                        "15", NewCardExchangeProduct(id: 100000015, masterId: 500090, name: "獅童真希")
                    },
                    {
                        "16", NewCardExchangeProduct(id: 100000016, masterId: 520000, name: "衛藤可奈美")
                    },
                    {
                        "17", NewCardExchangeProduct(id: 100000017, masterId: 530000, name: "御坂美琴")
                    },
                    {
                        "18", NewCardExchangeProduct(id: 100000017, masterId: 530010, name: "白井黒子")
                    },
                    {
                        "19", NewCardExchangeProduct(id: 100000019, masterId: 500100, name: "初春飾利")
                    },
                    {
                        "20", NewCardExchangeProduct(id: 100000020, masterId: 500110, name: "佐天涙子")
                    },
                    {
                        "21", NewCardExchangeProduct(id: 100000021, masterId: 500120, name: "御坂美琴")
                    },
                    {
                        "22", NewCardExchangeProduct(id: 100000022, masterId: 500130, name: "白井黒子")
                    }
                }
            )
        );
    }
    
    public static BoothResponse TradeItemResponse = new BoothResponse(
        exchange: new BoothExchange()
    );
    
    public static BoothEventResponse EventItemResponse = new BoothEventResponse();

    private static BoothExchangeProduct NewCardExchangeProduct(long id, long masterId, string name)
    {
        return new BoothExchangeProduct(
            id: id,
            masterId: masterId,
            fromContentId: 21,
            itemCategory: 1,
            point: 10000,
            name: name,
            quantity: 1
        );
    }
}