using Yuyuyui.PrivateServer.Responses;
using Yuyuyui.PrivateServer.Responses.Booth;

namespace Yuyuyui.PrivateServer.Constants;

public static class BoothConstants
{
    public static BoothResponse ExchangeItemResponse = new BoothResponse(
        exchange: new BoothExchange(
            id: 1,
            startAt: 1483228800,
            endAt: 1893456000,
            products: new Dictionary<string, BoothExchangeProduct>()
            {
                {
                    "1", 
                    new BoothExchangeProduct(
                        id: 1,
                        masterId: 500010,
                        fromContentId: 21,
                        itemCategory: 1,
                        point: 10000,
                        name: "源モモ",
                        quantity: 1
                    )
                },
                {
                    "2", 
                    new BoothExchangeProduct(
                        id: 2,
                        masterId: 500020,
                        fromContentId: 21,
                        itemCategory: 1,
                        point: 10000,
                        name: "半蔵門雪",
                        quantity: 1
                    )
                },
                {
                    "3", 
                    new BoothExchangeProduct(
                        id: 3,
                        masterId: 500030,
                        fromContentId: 21,
                        itemCategory: 1,
                        point: 10000,
                        name: "八千代命",
                        quantity: 1
                    )
                },
                {
                    "4", 
                    new BoothExchangeProduct(
                        id: 4,
                        masterId: 500040,
                        fromContentId: 21,
                        itemCategory: 1,
                        point: 10000,
                        name: "相模楓",
                        quantity: 1
                    )
                },
                {
                    "5", 
                    new BoothExchangeProduct(
                        id: 5,
                        masterId: 500050,
                        fromContentId: 21,
                        itemCategory: 1,
                        point: 10000,
                        name: "青葉初芽",
                        quantity: 1
                    )
                },
                {
                    "6", 
                    new BoothExchangeProduct(
                        id: 6,
                        masterId: 500060,
                        fromContentId: 21,
                        itemCategory: 1,
                        point: 10000,
                        name: "石川五恵",
                        quantity: 1
                    )
                }
            }
        )
    );
    
    public static BoothResponse TradeItemResponse = new BoothResponse(
        exchange: new BoothExchange()
    );
    
    public static BoothEventResponse EventItemResponse = new BoothEventResponse();
}