using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Yuyuyui.GK;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer
{
    public class LibGKLambda : ILibGK
    {
        private static string baseUrl = "https://ntnip45uf1.execute-api.ap-northeast-1.amazonaws.com/test";
        private static readonly MediaTypeWithQualityHeaderValue octetStream = new("application/octet-stream");

        private static async Task<byte[]> InvokeLambda(string type, string direction, byte[] inputData,
            string key = "", byte[]? iv = null, bool sessionKey = false)
        {
            List<string> queryParams = new List<string>(3);
            if (!string.IsNullOrEmpty(key))
                queryParams.Add($"key={key}");
            if (iv is {Length: > 0})
                queryParams.Add($"iv={HttpUtility.UrlEncode(iv)}");
            if (sessionKey)
                queryParams.Add($"session_key=1");

            string queryStr = "";
            if (queryParams.Count > 0)
                queryStr = $"?{string.Join("&", queryParams)}";

            string url = $"{baseUrl}/{type}/{direction}{queryStr}";

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            requestMessage.Content = new ByteArrayContent(inputData);
            requestMessage.Headers.Accept.Add(octetStream);
            requestMessage.Content.Headers.ContentType = octetStream;

            HttpResponseMessage response = await PrivateServer.HttpClient.SendAsync(requestMessage);
            byte[] decodedBytes = await response.Content.ReadAsByteArrayAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Utils.LogError(Resources.LOG_LIBGK_LAMBDA_FAILED + $"{type}/{direction}{queryStr}");
            }

            return decodedBytes;
        }

        public Task<byte[]> EncryptApi(byte[] inputData, string key = "", byte[]? iv = null, bool sessionKey = false)
        {
            return InvokeLambda("api", "encrypt", inputData, key, iv, sessionKey);
        }

        public Task<byte[]> DecryptApi(byte[] inputData, string key = "", byte[]? iv = null, bool sessionKey = false)
        {
            return InvokeLambda("api", "decrypt", inputData, key, iv, sessionKey);
        }

        public Task<byte[]> EncryptBin(byte[] inputData, string key = "", byte[]? iv = null)
        {
            return InvokeLambda("bin", "encrypt", inputData, key, iv);
        }

        public Task<byte[]> DecryptBin(byte[] inputData, string key = "", byte[]? iv = null)
        {
            return InvokeLambda("bin", "decrypt", inputData, key, iv);
        }
    }
}