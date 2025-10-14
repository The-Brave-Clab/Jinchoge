namespace Yuyuyui.PrivateServer.AWS;

public class LambdaInGameURLProvider : IInGameURLProvider
{
    public string MainPage => "https://app.yuyuyui.jp";
    public string APIBase => "https://app.yuyuyui.jp";
    public string Regulation => "https://app.yuyuyui.jp/api/v1/placeholder/regulation";
    public string Topics => "https://app.yuyuyui.jp/api/v1/placeholder/topics";
    public string Defects => "https://app.yuyuyui.jp/api/v1/placeholder/defects";
    public string Inquiry => "https://app.yuyuyui.jp/api/v1/placeholder/inquiry";
    public string Terms => "https://app.yuyuyui.jp/api/v1/placeholder/terms";
    public string Helps => "https://app.yuyuyui.jp/api/v1/placeholder/helps";
    public string OfficialLinks => "https://app.yuyuyui.jp";
}