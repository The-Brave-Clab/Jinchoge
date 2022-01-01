using System.Web;
using System.Net.Http.Headers;

namespace Yuyuyui.PrivateServer
{
    public class LibgkLambda
    {
        private static string baseUrl = "https://ntnip45uf1.execute-api.ap-northeast-1.amazonaws.com/test";
        private static readonly HttpClient client = new HttpClient();
        private static readonly MediaTypeWithQualityHeaderValue octet_stream = new MediaTypeWithQualityHeaderValue("application/octet-stream");

        public enum CryptType
        {
            Binary,
            API
        }

        public enum CryptDirection
        {
            Decrypt,
            Encrypt
        };

        private static string CryptToString(CryptType type, CryptDirection direction)
        {
            string typeStr = "";
            string directionStr = "";
            switch (type)
            {
                case CryptType.Binary:
                    typeStr = "bin";
                    break;
                case CryptType.API:
                    typeStr = "api";
                    break;
                default:
                    throw new ArgumentException("Unsupported CryptType");

            }

            switch (direction)
            {
                case CryptDirection.Decrypt:
                    directionStr = "decrypt";
                    break;
                case CryptDirection.Encrypt:
                    directionStr = "encrypt";
                    break;
                default:
                    throw new ArgumentException("Unsupported CryptDirection");
            }

            return $"/{typeStr}/{directionStr}";
        }

        public async static Task<byte[]> InvokeLambda(CryptType type, CryptDirection direction, byte[] inputData, string key = "", byte[]? iv = null, bool session_key = false)
        {
            string apiPath = CryptToString(type, direction);
            string url = baseUrl + apiPath;

            List<string> queryParams = new List<string>(3);
            if (!string.IsNullOrEmpty(key))
                queryParams.Add($"key={key}");
            if (iv != null && iv.Length > 0)
                queryParams.Add($"iv={HttpUtility.UrlEncode(iv)}");
            if (session_key)
                queryParams.Add($"session_key=1");

            if (queryParams.Count > 0)
                url += $"?{string.Join("&", queryParams)}";

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            requestMessage.Content = new ByteArrayContent(inputData);
            requestMessage.Headers.Accept.Add(octet_stream);
            requestMessage.Content.Headers.ContentType = octet_stream;

            HttpResponseMessage response = await client.SendAsync(requestMessage);
            byte[] decodedBytes = await response.Content.ReadAsByteArrayAsync();

            return decodedBytes;
        }
    }
}