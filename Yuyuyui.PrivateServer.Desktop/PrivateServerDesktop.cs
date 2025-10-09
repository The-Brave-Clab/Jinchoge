using System;
using System.Net.Http;

namespace Yuyuyui.PrivateServer.Desktop;

public static class PrivateServerDesktop
{
    public static readonly HttpClient HttpClient = new();

    public const string PRIVATE_LOCAL_API_SERVER = "private.yuyuyui.org";

    static PrivateServerDesktop()
    {
        HttpClient.DefaultRequestHeaders.Referrer = new Uri($"https://{PRIVATE_LOCAL_API_SERVER}");
    }
}