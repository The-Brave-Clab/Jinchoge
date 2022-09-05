namespace Yuyuyui.PrivateServer;

public class BoothExchange
{
    public long exchange_id { get; set; }
    public long start_at { get; set; } // unix time
    public long end_at { get; set; } // unix time
    public long? special_chapter_id { get; set; } = null;
    public IDictionary<string, BoothExchangeProduct> products = new Dictionary<string, BoothExchangeProduct>();
}

public class BoothResponse
{
    public BoothExchange exchange { get; set; }
}

public class BoothEventResponse
{
    public IList<BoothExchange> exchange { get; set; } = new List<BoothExchange>();
}