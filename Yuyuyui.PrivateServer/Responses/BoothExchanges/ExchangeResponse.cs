using Yuyuyui.PrivateServer.Responses.BoothExchanges.Exchange;

namespace Yuyuyui.PrivateServer.Responses.BoothExchanges;

public class ExchangeResponse
{
    public ExchangeResponse(PurchaseExchange exchange)
    {
        this.exchange = exchange;
    }

    public PurchaseExchange exchange { get; set; }
}