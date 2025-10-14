namespace Yuyuyui.PrivateServer.Desktop;

public class DesktopInGameURLProvider : IInGameURLProvider
{
    public string MainPage => $"http://{DesktopServerResourceProvider.PRIVATE_LOCAL_API_SERVER}";
    public string APIBase => "https://app.yuyuyui.jp";
    public string Regulation => $"http://{DesktopServerResourceProvider.PRIVATE_LOCAL_API_SERVER}";

    public string Topics =>
        $"http://{DesktopServerResourceProvider.PRIVATE_LOCAL_API_SERVER}/{ServerResources.RELEASE_NOTES_PATH}";
    public string Defects => $"http://{DesktopServerResourceProvider.PRIVATE_LOCAL_API_SERVER}";
    public string Inquiry => $"http://{DesktopServerResourceProvider.PRIVATE_LOCAL_API_SERVER}";
    public string Terms => $"http://{DesktopServerResourceProvider.PRIVATE_LOCAL_API_SERVER}";
    public string Helps => $"http://{DesktopServerResourceProvider.PRIVATE_LOCAL_API_SERVER}";
    public string OfficialLinks => $"http://{DesktopServerResourceProvider.PRIVATE_LOCAL_API_SERVER}";
}