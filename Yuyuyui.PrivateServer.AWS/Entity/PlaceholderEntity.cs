using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer.AWS;

public class PlaceholderEntity : BaseEntity<PlaceholderEntity>
{
    public PlaceholderEntity(
        Uri requestUri,
        string httpMethod,
        Dictionary<string, string> requestHeaders,
        byte[] requestBody,
        RouteConfig config)
        : base(requestUri, httpMethod, requestHeaders, requestBody, config)
    {
    }

    protected override Task ProcessRequest()
    {
        var param = GetPathParameter("param");

        responseBody = Encoding.UTF8.GetBytes($"{{\"category\":\"{param}\"}}");
        SetBasicResponseHeaders();

        return Task.CompletedTask;
    }
}