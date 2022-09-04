using Yuyuyui.PrivateServer.Responses.BoothExchanges;
using Yuyuyui.PrivateServer.Responses.BoothExchanges.Exchange;
using Yuyuyui.PrivateServer.Responses.BoothExchanges.Product;

namespace Yuyuyui.PrivateServer.Entity.Common.BoothExchanges;

public class CurrentBoothExchangeEntity : BaseEntity<CurrentBoothExchangeEntity>
{
    public CurrentBoothExchangeEntity(
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
        
        CurrentResponse currentResponse = new CurrentResponse(
            exchange: new CurrentExchange(product: new CurrentProduct(
                id: long.Parse(pathParameters.FirstOrDefault(
                    parameter => parameter.Key == "exchange_item").Value
                )
            ))
        );

        responseBody = Serialize(currentResponse);
        
        SetBasicResponseHeaders();
        return Task.CompletedTask;
    }
}