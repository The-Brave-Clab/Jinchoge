using Yuyuyui.PrivateServer.Responses.BoothExchanges.Product;

namespace Yuyuyui.PrivateServer.Responses.BoothExchanges.Exchange;

public class PurchaseExchange
{
    public PurchaseExchange(ExchangeProduct product)
    {
        this.product = product;
    }

    public ExchangeProduct product { get; set; }
}