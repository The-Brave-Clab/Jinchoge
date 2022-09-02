using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Utils = Yuyuyui.PrivateServer.Utils;
using ProxyUtils = Yuyuyui.PrivateServer.ProxyUtils;
using LibgkLambda = Yuyuyui.PrivateServer.LibgkLambda;

namespace Yuyuyui.AccountTransfer
{
    public abstract class EntityBase
    {
        public const string BASE_API_PATH = "/api/v1";
        public const string MIMETYPE_GK_JSON = "application/x-gk-json";
        public const string MIMETYPE_JSON = "application/json";

        public readonly string[] AcceptedHttpMethods;
        public readonly string HttpMethod;
        public readonly Uri RequestUri;
        protected readonly Dictionary<string, string> pathParameters;
        
        public Dictionary<string, string> PathParameters => pathParameters;

        public static bool HeaderContainsKey(HeaderCollection hearers, string headerKey)
        {
            return hearers.Any(header =>
                string.Equals(header.Name, headerKey, StringComparison.CurrentCultureIgnoreCase));
        }

        public static string GetRequestHeaderValue(HeaderCollection hearers, string headerKey)
        {
            foreach (var header in hearers.Where(header =>
                         string.Equals(header.Name, headerKey, StringComparison.CurrentCultureIgnoreCase)))
            {
                return header.Value;
            }

            return "";
        }

        protected static string StripApiPrefix(string apiPath)
        {
            return apiPath.StartsWith(BASE_API_PATH) ? apiPath.Substring(BASE_API_PATH.Length) : apiPath;
        }

        private static Dictionary<string, string>? ExtractPathParameters(string apiPathWithParameters,
            string apiPathReal)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            var orig = apiPathWithParameters.Split("/", StringSplitOptions.RemoveEmptyEntries);
            var real = apiPathReal.Split("/", StringSplitOptions.RemoveEmptyEntries);

            if (orig.Length != real.Length) return null;

            for (int i = 0; i < orig.Length; i++)
            {
                if (orig[i].StartsWith('{') && orig[i].EndsWith('}'))
                    result.Add(orig[i].Trim('{', '}'), real[i]);
                else if (orig[i] != real[i]) return null;
            }

            return result;
        }

        private static bool ApiPathMatch(string apiPathWithParameters, string apiPathReal)
        {
            return ExtractPathParameters(apiPathWithParameters, apiPathReal) != null;
        }

        public static Task<EntityBase?> FromRequestEvent(SessionEventArgs e)
        {
            string apiPath = StripApiPrefix(e.HttpClient.Request.RequestUri.AbsolutePath);

            foreach (var config in configs)
            {
                if (ApiPathMatch(config.Value.apiPath, apiPath) &&
                    config.Value.httpMethods.Contains(e.HttpClient.Request.Method))
                {
                    try
                    {
                        return Task.FromResult((EntityBase) TypeDescriptor.CreateInstance(
                            provider: null,
                            objectType: config.Key,
                            argTypes: new[]
                            {
                                typeof(Uri),
                                typeof(string),
                                typeof(Config)
                            },
                            args: new object[]
                            {
                                e.HttpClient.Request.RequestUri,
                                e.HttpClient.Request.Method,
                                config.Value
                            })!)!;
                    }
                    catch (Exception exception)
                    {
                        Utils.LogError(exception);
                        throw;
                    }
                }
            }

            return Task.FromResult<EntityBase?>(null); // not implemented api
        }

        public abstract void ProcessRequest(byte[] requestBody, 
            HeaderCollection requestHeaders, 
            ref AccountTransferProxyCallbacks.PlayerSession playerSession);
        public abstract void ProcessResponse(byte[] responseBody, 
            HeaderCollection responseHeaders, 
            ref AccountTransferProxyCallbacks.PlayerSession playerSession);

        protected static T? Deserialize<T>(byte[] data) where T : class
        {
            var stream = new MemoryStream(data);
            var reader = new StreamReader(stream, Encoding.UTF8);
            return JsonSerializer.Create().Deserialize(reader, typeof(T)) as T;
        }

        public EntityBase(Uri requestUri, string httpMethod, Config config)
        {
            AcceptedHttpMethods = config.httpMethods;
            HttpMethod = httpMethod;
            RequestUri = requestUri;

            pathParameters = ExtractPathParameters(config.apiPath, StripApiPrefix(requestUri.AbsolutePath))!;
        }

        private static readonly Dictionary<Type, Config> configs = new()
        {
            {
                typeof(SessionsEntity),
                new Config("/sessions", "POST")
            },
            {
                typeof(HeaderEntity),
                new Config("/my/header", "GET")
            },
            {
                typeof(UserInfoEntity),
                new Config("/users/{user_id}", "GET")
            },
        };
    }

    public struct Config
    {
        public Config(string apiPath, params string[] httpMethods)
        {
            this.apiPath = apiPath;
            this.httpMethods = httpMethods;
        }

        public readonly string apiPath;
        public readonly string[] httpMethods;
    }
}