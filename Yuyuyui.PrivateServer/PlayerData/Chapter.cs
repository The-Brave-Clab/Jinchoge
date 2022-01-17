namespace Yuyuyui.PrivateServer;

public class Chapter
{
    public long id { get; set; }
    public long master_id { get; set; }
    public bool new_released { get; set; }
    public bool completed { get; set; }
    public int kind { get; set; }
    public long start_at { get; set; }
    public long end_at { get; set; }
    public string detail_url { get; set; } = "";
    public long stack_point { get; set; }
    public bool locked { get; set; }
    public int available_user_level { get; set; }
}