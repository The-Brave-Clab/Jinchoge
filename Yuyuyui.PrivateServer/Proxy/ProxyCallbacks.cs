using System.Net;
using System.Text;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Exceptions;
using Titanium.Web.Proxy.Models;

namespace Yuyuyui.PrivateServer
{
    internal class ProxyCallbacks
    {
        private static async Task<bool> EchoService(SessionEventArgs e)
        {
            // Create a local echo service for downloading the cert file
            var request = e.HttpClient.Request;
            if (!e.IsHttps && request.Host == "download.cert")
            {
                if (request.RequestUri.AbsolutePath.Equals("/cert/pem", StringComparison.OrdinalIgnoreCase))
                {
                    // send the certificate
                    var headers = new Dictionary<string, HttpHeader>()
                    {
                        ["Content-Type"] = new HttpHeader("Content-Type", "application/x-x509-ca-cert"),
                        ["Content-Disposition"] = new HttpHeader("Content-Disposition", "inline; filename=titanium-ca-cert.pem")
                    };
                    e.Ok(File.ReadAllBytes("ca.cer"), headers, true);
                }
                else
                {
                    var headers = new Dictionary<string, HttpHeader>()
                    {
                        ["Content-Type"] = new HttpHeader("Content-Type", "text/html"),
                    };
                    e.Ok("<html><body><h1><a href=\"/cert/pem\">Download CA Certificate</a></h1></body></html>");
                }

                return true;
            }
            return false;
        }

        public static async Task OnRequest(object sender, SessionEventArgs e)
        {
            if (await EchoService(e)) return;

            if (!e.HttpClient.Request.RequestUri.Host.Contains("app.yuyuyui.jp"))
                return;
            
            EntityBase entity = EntityBase.FromRequestEvent(e);

            // read request headers
            string apiPath = e.HttpClient.Request.RequestUri.AbsolutePath;

            var requestHeaders = e.HttpClient.Request.Headers;

            var method = e.HttpClient.Request.Method.ToUpper();

            if (e.HttpClient.Request.ContentType != null)
            {
                try
                {
                    byte[] requestBodyBytes = await e.GetRequestBody();

                    var decodedBytes = await LibgkLambda.InvokeLambda(LibgkLambda.CryptType.API, LibgkLambda.CryptDirection.Decrypt, requestBodyBytes); //, currentKey, currentIV, currentSessionKey);
                    string decodedString = Encoding.UTF8.GetString(decodedBytes);
                    // userDataStr += $"|\tRequest \t{e.HttpClient.Request.ContentType}\t{requestBodyBytes.Length}";
                }
                catch (BodyNotFoundException)
                {

                }
            }
            //e.UserData = userData;

            // To cancel a request with a custom HTML content
            // Filter URL
            //if (e.HttpClient.Request.RequestUri.AbsoluteUri.Contains("google.com"))
            //{
            //    e.Ok("<!DOCTYPE html>" +
            //        "<html><body><h1>" +
            //        "Website Blocked" +
            //        "</h1>" +
            //        "<p>Blocked by titanium web proxy.</p>" +
            //        "</body>" +
            //        "</html>");
            //}

            //// Redirect example
            //if (e.HttpClient.Request.RequestUri.AbsoluteUri.Contains("wikipedia.org"))
            //{
            //    e.Redirect("https://www.paypal.com");
            //}
        }

        public static async Task OnResponse(object sender, SessionEventArgs e)
        {
            if (!e.HttpClient.Request.RequestUri.AbsoluteUri.Contains("app.yuyuyui.jp"))
                return;

            //RequestUserData userData = (RequestUserData)e.UserData!;

            // read response headers
            var responseHeaders = e.HttpClient.Response.Headers;

            if (e.HttpClient.Response.StatusCode == 200)
            {

                //if (!string.IsNullOrEmpty(e.HttpClient.Request.RequestUri.Query))
                //{
                    
                //}

                if (e.HttpClient.Response.ContentType != null)
                {
                    try
                    {
                        byte[] responseBodyBytes = await e.GetResponseBody();

                        var decodedBytes = await LibgkLambda.InvokeLambda(LibgkLambda.CryptType.API, LibgkLambda.CryptDirection.Decrypt, responseBodyBytes); //, currentKey, currentIV, currentSessionKey);
                        var decodedJsonStr = Encoding.UTF8.GetString(decodedBytes);
                    }
                    catch (BodyNotFoundException)
                    {

                    }
                    //e.SetResponseBody(bodyBytes);

                    //string body = await e.GetResponseBodyAsString();
                    //e.SetResponseBodyString(body);
                }
            }
        }

        // Allows overriding default certificate validation logic
        public static Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            // set IsValid to true/false based on Certificate Errors
            if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                e.IsValid = true;
            }

            return Task.CompletedTask;
        }

        // Allows overriding default client certificate selection logic during mutual authentication
        public static Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e)
        {
            // set e.clientCertificate to override
            return Task.CompletedTask;
        }

        public static Task OnBeforeTunnelConnect(object sender, TunnelConnectSessionEventArgs e)
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
