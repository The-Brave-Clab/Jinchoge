namespace Yuyuyui.PrivateServer.Responses.BoothExchanges.Product;

public class ExchangeProduct
{
    public ExchangeProduct(long id, int purchasedQuantity)
    {
        this.id = id;
        purchased_quantity = purchasedQuantity;
    }

    public long id { get; set; }
    public int purchased_quantity { get; set; }
}