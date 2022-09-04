namespace Yuyuyui.PrivateServer.Requests;

public class ExchangeBoothRequest
{
    public long exchange_booth_id { get; set; }
    public long exchange_booth_item_id { get; set; }
    public int count { get; set; }
    public int before_count { get; set; }
}