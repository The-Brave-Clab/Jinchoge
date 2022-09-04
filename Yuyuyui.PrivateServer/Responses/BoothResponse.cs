using Yuyuyui.PrivateServer.Responses.Booth;

namespace Yuyuyui.PrivateServer.Responses;

public class BoothResponse
{
    public BoothResponse(BoothExchange exchange)
    {
        this.exchange = exchange;
    }

    public BoothExchange exchange { get; set; }
}