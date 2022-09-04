namespace Yuyuyui.PrivateServer.Responses.BoothExchanges.Product;

public class CurrentProduct
{
    public CurrentProduct()
    {
    }

    public CurrentProduct(long id)
    {
        this.id = id;
    }

    public long id { get; set; }
    public int before_count { get; set; } = 1;
}