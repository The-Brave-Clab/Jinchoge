﻿using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Exceptions;
using Titanium.Web.Proxy.Helpers;
using Titanium.Web.Proxy.Models;

namespace Yuyuyui.PrivateServer
{
    public static class Proxy
    {
        public const int DEFAULT_PORT = 44460;

        private static ProxyServer? proxyServer = null;
        private static ExplicitProxyEndPoint? explicitEndPoint = null;

        public static ExplicitProxyEndPoint StartProxy(int port = DEFAULT_PORT)
        {
            proxyServer = new ProxyServer(false, false, false);
            // Use this very hacky way to disable systemProxySettingsManager on windows
            if (RunTime.IsWindows && !RunTime.IsUwpOnWindows)
            {
                var field = typeof(ProxyServer).GetField("<systemProxySettingsManager>k__BackingField",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                field?.SetValue(proxyServer, null);
            }
            proxyServer.EnableHttp2 = true;

            // locally trust root certificate used by this proxy 
            //proxyServer.CertificateManager.TrustRootCertificate(true);

            // Important
            proxyServer.CertificateManager.CertificateValidDays = 300;

            proxyServer.CertificateManager.EnsureRootCertificate();
            File.WriteAllBytes("ca.cer", proxyServer.CertificateManager.RootCertificate!.Export(X509ContentType.Cert));

            proxyServer.BeforeRequest += ProxyCallbacks.OnRequest;
            //proxyServer.BeforeResponse += ProxyCallbacks.OnResponse;
            proxyServer.ServerCertificateValidationCallback += ProxyCallbacks.OnCertificateValidation;
            proxyServer.ClientCertificateSelectionCallback += ProxyCallbacks.OnCertificateSelection;


            explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, port, true);

            // Fired when a CONNECT request is received
            explicitEndPoint.BeforeTunnelConnectRequest += ProxyCallbacks.OnBeforeTunnelConnect;
            explicitEndPoint.BeforeTunnelConnectResponse += ProxyCallbacks.OnBeforeTunnelConnect;

            // An explicit endpoint is where the client knows about the existence of a proxy
            // So client sends request in a proxy friendly manner
            proxyServer.AddEndPoint(explicitEndPoint);
            proxyServer.Start();

            return explicitEndPoint;
        }

        public static void Stop()
        {
            // Unsubscribe & Quit
            explicitEndPoint!.BeforeTunnelConnectRequest -= ProxyCallbacks.OnBeforeTunnelConnect;
            explicitEndPoint!.BeforeTunnelConnectResponse -= ProxyCallbacks.OnBeforeTunnelConnect;
            proxyServer!.BeforeRequest -= ProxyCallbacks.OnRequest;
            //proxyServer!.BeforeResponse -= ProxyCallbacks.OnResponse;
            proxyServer!.ServerCertificateValidationCallback -= ProxyCallbacks.OnCertificateValidation;
            proxyServer!.ClientCertificateSelectionCallback -= ProxyCallbacks.OnCertificateSelection;

            proxyServer!.Stop();
        }

        public static async Task<Tuple<Dictionary<string, string>, byte[]>> GetRequestHeadersAndBody(SessionEventArgs e)
        {
            byte[] requestBodyBytes = Array.Empty<byte>();
            Dictionary<string, string> headers =
                new Dictionary<string, string>(e.HttpClient.Request.Headers.Count());
            foreach (var header in e.HttpClient.Request.Headers)
            {
                if (!headers.Any(alreadyAddedHeader =>
                        string.Equals(alreadyAddedHeader.Key, header.Name,
                            StringComparison.CurrentCultureIgnoreCase)))
                {
                    headers.Add(header.Name, header.Value);
                }
            }

            if (e.HttpClient.Request.ContentType != null)
            {
                try
                {
                    requestBodyBytes = await e.GetRequestBody();
                }
                catch (BodyNotFoundException)
                {
                }
            }

            return new Tuple<Dictionary<string, string>, byte[]>(headers, requestBodyBytes);
        }
    }
}
