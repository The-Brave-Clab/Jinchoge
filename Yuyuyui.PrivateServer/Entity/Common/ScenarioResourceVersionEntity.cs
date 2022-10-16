using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer;

public class ScenarioResourceVersionEntity : GameResourceVersionEntity
{
    public ScenarioResourceVersionEntity(
        Uri requestUri,
        string httpMethod,
        Dictionary<string, string> requestHeaders,
        byte[] requestBody,
        RouteConfig config)
        : base(requestUri, httpMethod, requestHeaders, requestBody, config)
    {
    }

    protected override async Task ProcessRequest()
    {
        Utils.Log(Resources.LOG_PS_REDIRECT_API);

        string languageOption = "";

        var scenarioLanguage = Config.Get().InGame.ScenarioLanguage;
        if (Config.SupportedInGameScenarioLanguage.Contains(scenarioLanguage))
        {
            Utils.Log(string.Format(Resources.LOG_PS_SCENARIO_LANGUAGE, CultureInfo.GetCultureInfo(scenarioLanguage).DisplayName));
            if (scenarioLanguage != Config.SupportedInGameScenarioLanguage[0])
            {
                languageOption = $"/{scenarioLanguage}";
            }
        }

        HttpRequestMessage requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Get,
            new Uri($"https://{PrivateServer.PRIVATE_PUBLIC_API_SERVER}/test{RequestUri.AbsolutePath}{languageOption}"));

        //requestMessage.Content = new ByteArrayContent(Array.Empty<byte>());
        //requestMessage.Headers.Accept.Add(gk_json);
        //requestMessage.Content.Headers.ContentType = gk_json;  // The official server requires this.
        //requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", BASIC_AUTH_TOKEN);
        requestMessage.Headers.UserAgent.TryParseAdd(GetRequestHeaderValue("User-Agent"));
        requestMessage.Headers.Host = PrivateServer.PRIVATE_PUBLIC_API_SERVER;
        requestMessage.Headers.Connection.Add("Keep-Alive");
        requestMessage.Headers.AcceptEncoding.TryParseAdd("gzip");

        foreach (var header in requestHeaders
                     .Where(header =>
                         header.Key.StartsWith("X-", StringComparison.CurrentCultureIgnoreCase)))
        {
            requestMessage.Headers.Add(header.Key, header.Value);
        }

        HttpResponseMessage response = await PrivateServer.HttpClient.SendAsync(requestMessage);
        byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();

        // // the response from official server is in default key, we need to decrypt it
        // responseBody = await LibgkLambda.InvokeLambda(
        //     LibgkLambda.CryptType.API,
        //     LibgkLambda.CryptDirection.Decrypt,
        //     responseBytes);

        // the response from unofficial server is not encrypted
        responseBody = responseBytes;

        SetBasicResponseHeaders();
    }
}