using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Exceptions;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer
{
    public static class ProxyUtils
    {
        private const string CERT_PATH = "/cert/pem";
        private const string CERT_RESPONSE_FILE_NAME = "yuyuyui-private-server.pem";

        public static string LOCAL_CERT_FILE => Path.Combine(PrivateServer.BASE_DIR, "ca.cer");
        public static string LOCAL_PFX_FILE => Path.Combine(PrivateServer.BASE_DIR, "ca.pfx");

        private static readonly Assembly assembly = typeof(ProxyUtils).Assembly;
        private static readonly string[] assemblyResources = assembly.GetManifestResourceNames();

        public static bool WebService(SessionEventArgs e)
        {
            var request = e.HttpClient.Request;
            if (request.Host != PrivateServer.PRIVATE_LOCAL_API_SERVER ||
                e.HttpClient.Request.RequestUri.AbsolutePath.StartsWith(EntityBase.BASE_API_PATH))
                return false;

            if (request.RequestUri.AbsolutePath.Equals(CERT_PATH, StringComparison.OrdinalIgnoreCase))
            {
                // send the certificate
                var headers = new Dictionary<string, HttpHeader>
                {
                    ["Content-Type"] = new("Content-Type", "application/x-x509-ca-cert"),
                    ["Content-Disposition"] = new("Content-Disposition", $"inline; filename={CERT_RESPONSE_FILE_NAME}")
                };
                e.Ok(File.ReadAllBytes(LOCAL_CERT_FILE), headers, true);
                return true;
            }

            var path = request.RequestUri.AbsolutePath.Trim('/');
            if (string.IsNullOrEmpty(path)) path = "index";
            var expectedFile = $"webpages.{Resources.LAN_CODE}.{path.Replace('/', '.')}";
            var htmlHeaders = new Dictionary<string, HttpHeader>
            {
                ["Content-Type"] = new("Content-Type", "text/html; charset=utf-8"),
            };
            if (assemblyResources.Any(r => r.Contains(expectedFile, StringComparison.InvariantCultureIgnoreCase)))
            {
                var embeddedResource = assemblyResources.First(r =>
                    r.Contains(expectedFile, StringComparison.InvariantCultureIgnoreCase));
                using Stream stream = assembly.GetManifestResourceStream(embeddedResource)!;
                using StreamReader reader = new StreamReader(stream);
                var content = reader.ReadToEnd();
                e.Ok(content, htmlHeaders);
            }
            else
            {
                var embeddedResource = assemblyResources.First(r =>
                    r.Contains("404.html", StringComparison.InvariantCultureIgnoreCase));
                using Stream stream = assembly.GetManifestResourceStream(embeddedResource)!;
                using StreamReader reader = new StreamReader(stream);
                var content = reader.ReadToEnd();
                var response = new Response(Encoding.UTF8.GetBytes(content))
                    { StatusCode = 404, HttpVersion = HttpVersion.Version11 };
                response.Headers.AddHeaders(htmlHeaders);
                e.Respond(response);
            }

            return true;
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

        public static void ReissueCert()
        {
            if (File.Exists(LOCAL_PFX_FILE)) File.Delete(LOCAL_PFX_FILE);
            if (File.Exists(LOCAL_CERT_FILE)) File.Delete(LOCAL_CERT_FILE);
        }

        public static bool CertExists()
        {
            return File.Exists(LOCAL_PFX_FILE) && File.Exists(LOCAL_CERT_FILE);
        }
    }
}