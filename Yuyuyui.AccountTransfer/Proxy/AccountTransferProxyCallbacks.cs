using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Exceptions;
using Titanium.Web.Proxy.Http;
using Yuyuyui.GK;
using Yuyuyui.PrivateServer;

namespace Yuyuyui.AccountTransfer
{
    public class AccountTransferProxyCallbacks : IProxyCallbacks
    {
        public class PlayerSession
        {
            public PlayerProfile? player = null;
            public string sessionID = "";
            public string sessionKey = "";
        }

        private PlayerSession currentPlayerSession = new PlayerSession();
        
        private static string GetHeaderString(HeaderCollection header)
        {
            return header.Headers.Aggregate("", (current, httpHeader) => current + $"{httpHeader.Key}:{httpHeader.Value.Value}\n");
        }

        public async Task OnRequest(object sender, SessionEventArgs e)
        {
            if (ProxyUtils.WebService(e)) return;

            if (e.HttpClient.Request.RequestUri.Host.Contains("perf-events.cloud.unity3d.com"))
            {
                byte[] body = await e.GetRequestBody();
                string bodyStr = Encoding.UTF8.GetString(body);
                Utils.LogWarning(bodyStr);
            }

            if (!e.HttpClient.Request.RequestUri.Host.Contains("app.yuyuyui.jp"))
                return;
            
            EntityBase? entity = await EntityBase.FromRequestEvent(e);
            
            // For not implemented APIs, we do nothing here.
            if (entity == null)
                return;
            
            byte[] decodedBytes = Array.Empty<byte>();
            
            if (e.HttpClient.Request.ContentType != null)
            {
                try
                {
                    byte[] requestBodyBytes = await e.GetRequestBody();
                    if (e.HttpClient.Request.ContentType == "application/x-gk-json")
                    {
                        decodedBytes =
                            Config.Get().Security.UseOnlineDecryption
                                ? LibGK<LibGKLambda>.Execute(CryptType.API, CryptDirection.Decrypt, requestBodyBytes,
                                    currentPlayerSession.sessionKey, null,
                                    !string.IsNullOrEmpty(currentPlayerSession.sessionKey))
                                : LibGK<GoalKeeper>.Execute(CryptType.API, CryptDirection.Decrypt, requestBodyBytes,
                                    currentPlayerSession.sessionKey, null,
                                    !string.IsNullOrEmpty(currentPlayerSession.sessionKey));
                    }
                    else
                    {
                        decodedBytes = requestBodyBytes;
                    }
                }
                catch (BodyNotFoundException)
                {

                }
            }

            lock (currentPlayerSession)
            {
                entity.ProcessRequest(decodedBytes, e.HttpClient.Request.Headers, ref currentPlayerSession);
            }
            e.UserData = entity;
        }

        public async Task OnResponse(object sender, SessionEventArgs e)
        {
            EntityBase? entity = e.UserData as EntityBase;

            if (entity == null)
                return;
            
            byte[] decodedBytes = Array.Empty<byte>();
            
            if (e.HttpClient.Response.ContentType != null)
            {
                try
                {
                    byte[] responseBodyBytes = await e.GetResponseBody();
                    if (e.HttpClient.Response.ContentType == "application/x-gk-json")
                    {
                        decodedBytes =
                            Config.Get().Security.UseOnlineDecryption
                                ? LibGK<LibGKLambda>.Execute(CryptType.API, CryptDirection.Decrypt, responseBodyBytes,
                                    currentPlayerSession.sessionKey, null,
                                    !string.IsNullOrEmpty(currentPlayerSession.sessionKey))
                                : LibGK<GoalKeeper>.Execute(CryptType.API, CryptDirection.Decrypt, responseBodyBytes,
                                    currentPlayerSession.sessionKey, null,
                                    !string.IsNullOrEmpty(currentPlayerSession.sessionKey));
                    }
                    else
                    {
                        decodedBytes = responseBodyBytes;
                    }
                }
                catch (BodyNotFoundException)
                {

                }
            }

            lock (currentPlayerSession)
            {
                if (entity.TransferTask == TransferProgress.TaskType.Count_DoNotUse ||
                    TransferProgress.IsCompleted(entity.TransferTask))
                    return;
                entity.ProcessResponse(decodedBytes, e.HttpClient.Response.Headers, ref currentPlayerSession);
                TransferProgress.Complete(entity.TransferTask);
            }
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

            e.DecryptSsl = e.HttpClient.Request.RequestUri.Host.Contains("app.yuyuyui.jp");

            return Task.CompletedTask;
        }
    }
}
