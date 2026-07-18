using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace Yuyuyui.PrivateServer
{
    public class PrivateServerProxyCallbacks : IProxyCallbacks
    {
        public async Task OnRequest(object sender, SessionEventArgs e)
        {
            if (ProxyUtils.WebService(e)) return;

            if (!e.HttpClient.Request.RequestUri.Host.Contains(PrivateServer.OFFICIAL_API_SERVER) &&
                !e.HttpClient.Request.RequestUri.Host.Contains(PrivateServer.PRIVATE_LOCAL_API_SERVER))
                return;
            
            EntityBase entity = await EntityBase.FromRequestEvent(e);
            
            try
            {
                await entity.Process();
            }
            catch (APIErrorException apiError)
            {
                var headersAndBody = await ProxyUtils.GetRequestHeadersAndBody(e);
                
                entity = new RequestErrorEntity(
                    apiError.errorCode,
                    $"{apiError.body}",
                    e.HttpClient.Request.RequestUri,
                    e.HttpClient.Request.Method,
                    new RouteConfig(entity.RequestUri.AbsolutePath, e.HttpClient.Request.Method),
                    headersAndBody.Item1,
                    headersAndBody.Item2,
                    $"{apiError.body}");
                await entity.Process();
            }
            catch (Exception exception)
            {
                Utils.LogError(exception.ToString());

                entity = new RequestErrorEntity(
                    "A1321",
                    "Internal private server error",
                    e.HttpClient.Request.RequestUri,
                    e.HttpClient.Request.Method,
                    new RouteConfig(entity.RequestUri.AbsolutePath, e.HttpClient.Request.Method),
                    new Dictionary<string, string>(),
                    Array.Empty<byte>(),
                    exception.Message);
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
                || e.HttpClient.Request.RequestUri.Host.Contains(PrivateServer.PRIVATE_LOCAL_API_SERVER);

            return Task.CompletedTask;
        }
    }
}
