﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Exceptions;
using Titanium.Web.Proxy.Models;

namespace Yuyuyui.PrivateServer
{
    public static class ProxyUtils
    {
        private const string CERT_HOST = "download.cert";
        private const string CERT_PATH = "/cert/pem";
        private const string CERT_RESPONSE_FILE_NAME = "yuyuyui-private-server.pem";

        public static string LOCAL_CERT_FILE => Path.Combine(PrivateServer.BASE_DIR, "ca.cer");
        public static string LOCAL_PFX_FILE => Path.Combine(PrivateServer.BASE_DIR, "ca.pfx");

        public static bool EchoService(SessionEventArgs e)
        {
            // Create a local echo service for downloading the cert file
            var request = e.HttpClient.Request;
            if (!e.IsHttps && request.Host == CERT_HOST)
            {
                if (request.RequestUri.AbsolutePath.Equals(CERT_PATH, StringComparison.OrdinalIgnoreCase))
                {
                    // send the certificate
                    var headers = new Dictionary<string, HttpHeader>
                    {
                        ["Content-Type"] = new("Content-Type", "application/x-x509-ca-cert"),
                        ["Content-Disposition"] = new("Content-Disposition", $"inline; filename={CERT_RESPONSE_FILE_NAME}")
                    };
                    e.Ok(File.ReadAllBytes(LOCAL_CERT_FILE), headers, true);
                }
                else
                {
                    var headers = new Dictionary<string, HttpHeader>
                    {
                        ["Content-Type"] = new("Content-Type", "text/html"),
                    };
                    e.Ok($"<html><body><h1><a href=\"{CERT_PATH}\">Download CA Certificate</a></h1></body></html>",
                        headers);
                }

                return true;
            }

            return false;
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