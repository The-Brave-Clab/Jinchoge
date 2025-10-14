namespace Yuyuyui.PrivateServer;

public interface IInGameURLProvider
{
    string MainPage { get; }
    string APIBase { get; }
    string Regulation { get; }
    string Topics { get; }
    string Defects { get; }
    string Inquiry { get; }
    string Terms { get; }
    string Helps { get; }
    string OfficialLinks { get; }
}