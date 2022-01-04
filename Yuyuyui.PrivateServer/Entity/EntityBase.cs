using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Exceptions;

namespace Yuyuyui.PrivateServer
{
    public abstract class EntityBase
    {
        public const string BASE_API_PATH = "/api/v1";
        public const string MIMETYPE_GK_JSON = "application/x-gk-json";
        public const string MIMETYPE_JSON = "application/json";

        public readonly string HttpMethod;
        public readonly Uri RequestUri;
        protected byte[] requestBody;
        protected readonly Dictionary<string, string> requestHeaders;
        protected readonly Dictionary<string, string> pathParameters;
        protected byte[] responseBody;
        protected Dictionary<string, string> responseHeaders;

        
        public byte[] RequestBody => requestBody;
        public Dictionary<string, string> RequestHeaders => requestHeaders;
        public Dictionary<string, string> PathParameters => pathParameters;
        public byte[] ResponseBody => responseBody;
        public Dictionary<string, string> ResponseHeaders => responseHeaders;

        public string GetRequestHeaderValue(string headerKey)
        {
            string headerKeyLower = headerKey.ToLower();
            if (requestHeaders.ContainsKey(headerKeyLower))
                return requestHeaders[headerKeyLower];
            return "";
        }

        public string GetPathParameter(string key)
        {
            return pathParameters[key];
        }

        protected bool HasRequestBody()
        {
            return requestBody.Length == 0;
        }
        
        protected static string StripApiPrefix(string apiPath)
        {
            return apiPath.StartsWith(BASE_API_PATH) ? apiPath.Substring(BASE_API_PATH.Length) : apiPath;
        }

        private static Dictionary<string, string>? ExtractPathParameters(string apiPathWithParameters, string apiPathReal)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            
            var orig = apiPathWithParameters.Split("/", StringSplitOptions.RemoveEmptyEntries);
            var real = apiPathReal.Split("/", StringSplitOptions.RemoveEmptyEntries);

            if (orig.Length != real.Length) return null;

            for (int i = 0; i < orig.Length; i++)
            {
                if (orig[i].StartsWith('{') && orig[i].EndsWith('}'))
                    result.Add(orig[i].Trim('{', '}'), real[i]);
                else
                    if (orig[i] != real[i]) return null;
            }

            return result;
        }

        private static bool ApiPathMatch(string apiPathWithParameters, string apiPathReal)
        {
            return ExtractPathParameters(apiPathWithParameters, apiPathReal) != null;
        }

        public static async Task<EntityBase> FromRequestEvent(SessionEventArgs e)
        {
            string apiPath = StripApiPrefix(e.HttpClient.Request.RequestUri.AbsolutePath);

            foreach (var config in configs)
            {
                if (ApiPathMatch(config.Value.apiPath, apiPath) &&
                    config.Value.httpMethod == e.HttpClient.Request.Method)
                {
                    try
                    {
                        Dictionary<string, string> headers =
                            new Dictionary<string, string>(e.HttpClient.Request.Headers.Count());
                        foreach (var header in e.HttpClient.Request.Headers)
                        {
                            string headerKey = header.Name.ToLower();
                            if (!headers.ContainsKey(headerKey))
                                headers.Add(headerKey, header.Value);
                        }

                        byte[] requestBodyBytes = Array.Empty<byte>();
                        
                        if (e.HttpClient.Request.ContentType != null)
                        {
                            try
                            {
                                requestBodyBytes = await e.GetRequestBody();
                                // userDataStr += $"|\tRequest \t{e.HttpClient.Request.ContentType}\t{requestBodyBytes.Length}";
                            }
                            catch (BodyNotFoundException)
                            {

                            }
                        }
                        
                        return (EntityBase) TypeDescriptor.CreateInstance(
                            provider: null,
                            objectType: config.Key,
                            argTypes: new[] {typeof(Uri), typeof(Dictionary<string, string>), typeof(byte[]), typeof(Config)},
                            args: new object[] {e.HttpClient.Request.RequestUri, headers, requestBodyBytes, config.Value})!;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        throw;
                    }
                }
            }

            return new RequestErrorEntity("S2000", $"API Not Implemented:\n\n{apiPath}",
                e.HttpClient.Request.RequestUri, new Config(apiPath, e.HttpClient.Request.Method)); // error type
        }

        protected abstract Task ProcessRequest();

        public async Task Process()
        {
            if (requestBody.Length > 0)
            {
                if (GetRequestHeaderValue("Content-Type").ToLower() == MIMETYPE_GK_JSON)
                {
                    bool hasSessionCookie = this.GetSessionFromCookie(out var session);
                    if (!hasSessionCookie)
                    {
                        requestBody = await LibgkLambda.InvokeLambda(
                            LibgkLambda.CryptType.API, 
                            LibgkLambda.CryptDirection.Decrypt, 
                            requestBody); //, currentKey, currentIV, currentSessionKey);
                    }
                    else
                    {
                        Console.WriteLine(session.sessionKey);
                        requestBody = await LibgkLambda.InvokeLambda(
                            LibgkLambda.CryptType.API, 
                            LibgkLambda.CryptDirection.Decrypt, 
                            requestBody, 
                            session.sessionKey, sessionKey: true);
                    }
                }
            }
            
            await ProcessRequest();
        }
        
        protected static T? Deserialize<T>(byte[] data) where T : class
        {
            var stream = new MemoryStream(data);
            var reader = new StreamReader(stream, Encoding.UTF8);
            return JsonSerializer.Create().Deserialize(reader, typeof(T)) as T;
        }

        protected static byte[] Serialize<T>(T obj) where T : class
        {
            string str = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(str);
        }

        protected void SetBasicResponseHeaders(string sessionId = "", bool isGk = false)
        {
            responseHeaders.Add("Content-Type", isGk ? MIMETYPE_GK_JSON : MIMETYPE_JSON);
            responseHeaders.Add("Content-Length", $"{responseBody.Length}");
            if (!string.IsNullOrEmpty(sessionId))
                responseHeaders.Add("Set-Cookie", $"_session_id={sessionId}");
        }

        public EntityBase(Uri requestUri, Dictionary<string, string> requestHeaders, byte[] requestBody, Config config)
        {
            HttpMethod = config.httpMethod;
            RequestUri = requestUri;
            this.requestHeaders = requestHeaders;
            this.requestBody = requestBody;
            responseBody = Array.Empty<byte>();
            responseHeaders = new Dictionary<string, string>();

            pathParameters = ExtractPathParameters(config.apiPath, StripApiPrefix(requestUri.AbsolutePath))!;
        }

        public static readonly Dictionary<Type, Config> configs = new()
        {
            {
                typeof(ArticleEntity),
                new Config("/articles", "GET")
            },
            {
                typeof(RegistrationsEntity),
                new Config("/registrations", "POST")
            },
            {
                typeof(SessionsEntity),
                new Config("/sessions", "POST")
            },
            {
                typeof(RequirementVersionEntity),
                new Config("/requirement_version", "GET")
            },
        };
    }

    public struct Config
    {
        public Config(string apiPath, string httpMethod)
        {
            this.apiPath = apiPath;
            this.httpMethod = httpMethod;
        }

        public readonly string apiPath;
        public readonly string httpMethod;
    }
}