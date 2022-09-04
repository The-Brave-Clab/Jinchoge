namespace Yuyuyui.PrivateServer.Responses.Booth;

public class BoothExchange
{
    public BoothExchange()
    {
    }

    public BoothExchange(long exchangeId, long startAt, long endAt, Dictionary<string, BoothExchangeProduct> products)
    {
        this.exchange_id = exchangeId;
        start_at = startAt;
        end_at = endAt;
        this.products = products;
    }

    public long exchange_id { get; set; }
    // start_at = Unix Time
    public long start_at { get; set; }
    // end_at = Unix Time
    public long end_at { get; set; }
    // special Chapter ID, default null
    public long? special_chapter_id { get; set; } = null;
    public Dictionary<string, BoothExchangeProduct> products = new Dictionary<string, BoothExchangeProduct>();
}