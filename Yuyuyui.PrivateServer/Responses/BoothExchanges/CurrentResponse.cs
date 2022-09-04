using Yuyuyui.PrivateServer.Responses.BoothExchanges.Exchange;

namespace Yuyuyui.PrivateServer.Responses.BoothExchanges;

public class CurrentResponse
{
    public CurrentResponse(CurrentExchange exchange)
    {
        this.exchange = exchange;
    }

    public CurrentExchange exchange { get; set; }
}