using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Exceptions;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace Yuyuyui.PrivateServer.Desktop;

public class PrivateServerProxyCallbacks : IProxyCallbacks
{
    public async Task OnRequest(object sender, SessionEventArgs e)
    {
        if (ProxyUtils.WebService(e)) return;

        if (e.HttpClient.Request.RequestUri.Host.Contains("perf-events.cloud.unity3d.com"))
        {
            byte[] body = await e.GetRequestBody();
            string bodyStr = Encoding.UTF8.GetString(body);
            Utils.LogWarning(bodyStr);
        }

        if (!e.HttpClient.Request.RequestUri.Host.Contains(PrivateServer.OFFICIAL_API_SERVER) &&
            !e.HttpClient.Request.RequestUri.Host.Contains(PrivateServer.PRIVATE_LOCAL_API_SERVER))
            return;

        EventArgs args = new()
        {
            requestUri = e.HttpClient.Request.RequestUri,
            requestMethod = e.HttpClient.Request.Method,
            header = new(e.HttpClient.Request.Headers.Count()),
            requestBody = []
        };

        foreach (var header in e.HttpClient.Request.Headers)
        {
            if (!args.header.Any(alreadyAddedHeader =>
                    string.Equals(alreadyAddedHeader.Key, header.Name,
                        StringComparison.CurrentCultureIgnoreCase)))
            {
                args.header.Add(header.Name, header.Value);
            }
        }

        if (e.HttpClient.Request.ContentType != null)
        {
            try
            {
                args.requestBody = await e.GetRequestBody();
            }
            catch (BodyNotFoundException)
            {
            }
        }
            
        EntityBase entity = EntityBase.FromEventArgs(args);
            
        try
        {
            await entity.Process();
        }
        catch (APIErrorException apiError)
        {
            entity = new RequestErrorEntity(
                apiError.errorCode,
                $"{apiError.body}",
                args.requestUri,
                args.requestMethod,
                new RouteConfig(args.requestUri.AbsolutePath, args.requestMethod),
                args.header,
                args.requestBody,
                $"{apiError.body}");
            await entity.Process();
        }

        byte[] responseBody = entity.ResponseBody;
        Dictionary<string, string> responseHeaders = entity.ResponseHeaders;

        if (entity.GetType() == typeof(RequestErrorEntity))
        {
            var errorEntity = (RequestErrorEntity)entity;
            e.Respond(new Response(entity.ResponseBody) {StatusCode = errorEntity.StatusCode, HttpVersion = HttpVersion.Version11});
        }
        else
        {
            e.Ok(responseBody, responseHeaders.Select(p => new HttpHeader(p.Key, p.Value)));
        }
    }

    public Task OnResponse(object sender, SessionEventArgs e)
    {
        return Task.CompletedTask;
    }

    // Allows overriding default certificate validation logic
    public Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
    {
        // set IsValid to true/false based on Certificate Errors
        if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
        {
            e.IsValid = true;
        }

        return Task.CompletedTask;
    }

    // Allows overriding default client certificate selection logic during mutual authentication
    public Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e)
    {
        // set e.clientCertificate to override
        return Task.CompletedTask;
    }

    public Task OnBeforeTunnelConnect(object sender, TunnelConnectSessionEventArgs e)
    {
        var clientLocalIp = e.ClientLocalEndPoint.Address;
        if (!clientLocalIp.Equals(IPAddress.Loopback) && !clientLocalIp.Equals(IPAddress.IPv6Loopback))
        {
            e.HttpClient.UpStreamEndPoint = new IPEndPoint(clientLocalIp, 0);
        }

        e.DecryptSsl = e.HttpClient.Request.RequestUri.Host.Contains(PrivateServer.OFFICIAL_API_SERVER)
                       || e.HttpClient.Request.RequestUri.Host.Contains(PrivateServer.PRIVATE_LOCAL_API_SERVER)
                       || e.HttpClient.Request.RequestUri.Host.Contains("perf-events.cloud.unity3d.com");

        return Task.CompletedTask;
    }
}