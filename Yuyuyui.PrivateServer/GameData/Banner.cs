namespace Yuyuyui.PrivateServer
{
    public class Banner
    {
        public long image_id { get; set; }
        public string transition_screen_kind { get; set; } = "";
        public string transition_url { get; set; } = "";
        public int available_user_level { get; set; }
    }
}
