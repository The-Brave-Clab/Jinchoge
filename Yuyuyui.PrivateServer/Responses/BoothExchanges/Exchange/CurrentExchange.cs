using Yuyuyui.PrivateServer.Responses.BoothExchanges.Product;

namespace Yuyuyui.PrivateServer.Responses.BoothExchanges.Exchange;

public class CurrentExchange
{
    public CurrentExchange(CurrentProduct product)
    {
        this.product = product;
    }

    public CurrentProduct product { get; set; }
}