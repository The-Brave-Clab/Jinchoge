using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;
using Markdig;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer.Desktop;

public static class ProxyUtils
{
    private const string CERT_PATH = "/cert/pem";
    private const string CERT_RESPONSE_FILE_NAME = "yuyuyui-private-server.pem";

    public static string LOCAL_CERT_FILE => Path.Combine(FileSystemData.BASE_DIR, "ca.cer");
    public static string LOCAL_PFX_FILE => Path.Combine(FileSystemData.BASE_DIR, "ca.pfx");

    public static bool WebService(SessionEventArgs e)
    {
        var request = e.HttpClient.Request;
        if (request.Host != DesktopServerResourceProvider.PRIVATE_LOCAL_API_SERVER ||
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

        if (path.Equals(ServerResources.RELEASE_NOTES_PATH, StringComparison.OrdinalIgnoreCase))
        {
            var markdownFile = $"documents.{Resources.LAN_CODE}.{ServerResources.RELEASE_NOTES_PATH}.md";
            var markdown = ServerResources.ReadAllTextFromAssemblyResources(markdownFile);
            var htmlTemplateFile = $"documents.release-notes-template.html";
            var htmlTemplate = ServerResources.ReadAllTextFromAssemblyResources(htmlTemplateFile);

            var body = Markdown.ToHtml(markdown);
            var title = Resources.HTML_RELEASE_NOTES_TITLE;

            var html = htmlTemplate.Replace("{{ title }}", title).Replace("{{ body }}", body);
            e.Ok(html, htmlHeaders);
        }
        else if (DesktopResources.EmbeddedResources
                 .Any(r => r.Contains(expectedFile, StringComparison.InvariantCultureIgnoreCase)))
        {
            var content = DesktopResources.ReadAllTextFromAssemblyResources(expectedFile);
            e.Ok(content, htmlHeaders);
        }
        else
        {
            var page404 = DesktopResources.EmbeddedResources.First(r =>
                r.Contains("404.html", StringComparison.InvariantCultureIgnoreCase));
            var content = DesktopResources.ReadAllTextFromAssemblyResources(page404);
            var response = new Response(Encoding.UTF8.GetBytes(content))
                { StatusCode = 404, HttpVersion = HttpVersion.Version11 };
            response.Headers.AddHeaders(htmlHeaders);
            e.Respond(response);
        }

        return true;
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