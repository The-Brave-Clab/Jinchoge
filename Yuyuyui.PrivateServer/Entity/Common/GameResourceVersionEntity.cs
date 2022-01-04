using System.Net.Http.Headers;
using System.Text;

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
            Config config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
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

            Utils.LogWarning("Redirected to official API Server!");

            // requestBody is decrypted, before we send it we need to encrypt it back, with default key
            var encryptedBody = await LibgkLambda.InvokeLambda(
                LibgkLambda.CryptType.API,
                LibgkLambda.CryptDirection.Encrypt,
                requestBody); //, currentKey, currentIV, currentSessionKey);

            HttpRequestMessage requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, RequestUri);

            requestMessage.Content = new ByteArrayContent(encryptedBody);
            //requestMessage.Headers.Accept.Add(gk_json);
            requestMessage.Content.Headers.ContentType = gk_json;
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", BASIC_AUTH_TOKEN);
            requestMessage.Headers.UserAgent.TryParseAdd(GetRequestHeaderValue("User-Agent"));
            requestMessage.Headers.Host = "app.yuyuyui.jp";
            requestMessage.Headers.Connection.Add("Keep-Alive");
            requestMessage.Headers.AcceptEncoding.TryParseAdd("gzip");

            foreach (var header in requestHeaders)
            {
                if (!header.Key.StartsWith("X-", StringComparison.CurrentCultureIgnoreCase))
                    continue;
                requestMessage.Content.Headers.Add(header.Key, header.Value);
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