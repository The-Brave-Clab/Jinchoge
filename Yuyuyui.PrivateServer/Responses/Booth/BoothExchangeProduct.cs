namespace Yuyuyui.PrivateServer;

public class BoothExchangeProduct
{
    public long id { get; set; }
    public long master_id { get; set; }
    public long from_content_id { get; set; }
    public long? special_chapter_id { get; set; } = null;
    /*
     * item_category:
     * 1 = Card
     * 4 = Evolution Item
     * 5 = Botamochi series
     * 15 = 神樹の恵み
     */
    public int item_category { get; set; }
    public int purchased_quantity { get; set; } = 0;
    public int purchase_limit_quantity { get; set; } = 99;
    public int point { get; set; }
    public string name { get; set; }
    public int quantity { get; set; } = 1;
    public bool have_story { get; set; } = false;
}