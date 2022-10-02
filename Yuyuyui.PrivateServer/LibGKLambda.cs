using System.Net;
using System.Web;
using System.Net.Http.Headers;
using Yuyuyui.GK;

namespace Yuyuyui.PrivateServer
{
    public class LibGKLambda : ILibGK
    {
        private static string baseUrl = "https://ntnip45uf1.execute-api.ap-northeast-1.amazonaws.com/test";
        private static readonly MediaTypeWithQualityHeaderValue octetStream = new("application/octet-stream");

        private enum CryptType
        {
            Binary,
            API
        }

        private enum CryptDirection
        {
            Decrypt,
            Encrypt
        };

        private static string CryptToString(CryptType type, CryptDirection direction)
        {
            string typeStr = type switch
            {
                CryptType.Binary => "bin",
                CryptType.API    => "api",
                _ => throw new ArgumentException("Unsupported CryptType")
            };

            string directionStr = direction switch
            {
                CryptDirection.Decrypt => "decrypt",
                CryptDirection.Encrypt => "encrypt",
                _ => throw new ArgumentException("Unsupported CryptDirection")
            };

            return $"/{typeStr}/{directionStr}";
        }

        private static async Task<byte[]> InvokeLambda(CryptType type, CryptDirection direction, byte[] inputData,
            string key = "", byte[]? iv = null, bool sessionKey = false)
        {
            string apiPath = CryptToString(type, direction);

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

            string url = $"{baseUrl}{apiPath}{queryStr}";

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            requestMessage.Content = new ByteArrayContent(inputData);
            requestMessage.Headers.Accept.Add(octetStream);
            requestMessage.Content.Headers.ContentType = octetStream;

            HttpResponseMessage response = await PrivateServer.HttpClient.SendAsync(requestMessage);
            byte[] decodedBytes = await response.Content.ReadAsByteArrayAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Utils.LogError($"Libgk Call Failed! {apiPath}{queryStr}");
            }

            return decodedBytes;
        }

        public byte[] EncryptApi(byte[] inputData, string key = "", byte[]? iv = null, bool sessionKey = false)
        {
            var task = Task.Run(() => InvokeLambda(CryptType.API, CryptDirection.Encrypt, inputData, key, iv, sessionKey));
            task.Wait();
            return task.Result;
        }

        public byte[] DecryptApi(byte[] inputData, string key = "", byte[]? iv = null, bool sessionKey = false)
        {
            var task = Task.Run(() => InvokeLambda(CryptType.API, CryptDirection.Decrypt, inputData, key, iv, sessionKey));
            task.Wait();
            return task.Result;
        }

        public byte[] EncryptBin(byte[] inputData, string key = "", byte[]? iv = null)
        {
            var task = Task.Run(() => InvokeLambda(CryptType.Binary, CryptDirection.Encrypt, inputData, key, iv));
            task.Wait();
            return task.Result;
        }

        public byte[] DecryptBin(byte[] inputData, string key = "", byte[]? iv = null)
        {
            var task = Task.Run(() => InvokeLambda(CryptType.Binary, CryptDirection.Decrypt, inputData, key, iv));
            task.Wait();
            return task.Result;
        }
    }
}