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

    public static void SetPrivateServerURIRewriter()
    {
        PrivateServer.RequestURIRewriter = uri =>
        {
            // replace the host with the private server host
            if (!uri.Host.Equals(PrivateServer.OFFICIAL_API_SERVER, StringComparison.OrdinalIgnoreCase))
                return uri;

            UriBuilder builder = new(uri)
            {
                Host = PRIVATE_LOCAL_API_SERVER
            };
            return builder.Uri;

        };
    }
}