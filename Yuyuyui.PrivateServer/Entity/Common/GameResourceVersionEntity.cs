using System.Net;
using System.Net.Http.Headers;

namespace Yuyuyui.PrivateServer
{
    public class GameResourceVersionEntity : BaseEntity<GameResourceVersionEntity>
    {
        private static readonly MediaTypeWithQualityHeaderValue gk_json = new("application/x-gk-json");
        public const string BASIC_AUTH_TOKEN = "eXV5dXl1ZGV2OjRjOWFwazc2ZXd4cnRidzU=";

        public GameResourceVersionEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config,
            IPEndPoint localEndPoint)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config, localEndPoint)
        {
        }

        protected override async Task ProcessRequest()
        {
            // For now, we route this api call to the official server
            Utils.Log("Path parameters:");
            foreach (var pathParameter in pathParameters)
            {
                Utils.Log($"\t{pathParameter.Key} = {pathParameter.Value}");
            }

            Utils.LogWarning("Redirected to the official API Server!");

            HttpRequestMessage requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, RequestUri);

            requestMessage.Content = new ByteArrayContent(Array.Empty<byte>());
            //requestMessage.Headers.Accept.Add(gk_json);
            requestMessage.Content.Headers.ContentType = gk_json;  // The official server requires this.
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", BASIC_AUTH_TOKEN);
            requestMessage.Headers.UserAgent.TryParseAdd(GetRequestHeaderValue("User-Agent"));
            requestMessage.Headers.Host = "app.yuyuyui.jp";
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
            
            // the response is in default key, we need to decrypt it
            responseBody = await LibgkLambda.InvokeLambda(
                LibgkLambda.CryptType.API,
                LibgkLambda.CryptDirection.Decrypt,
                responseBytes);

            SetBasicResponseHeaders();
        }
    }
}