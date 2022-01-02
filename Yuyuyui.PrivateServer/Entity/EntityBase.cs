using System.ComponentModel;
using Titanium.Web.Proxy.EventArguments;

namespace Yuyuyui.PrivateServer
{
    public class EntityBase
    {
        public const string BASE_API_PATH = "/api/v1";
        
        protected SessionEventArgs? sessionEventArgs = null;

        protected string? httpMethod;
        public string? HttpMethod => httpMethod;

        protected Uri? requestUri;
        public Uri? RequestUri => requestUri;

        protected Dictionary<string, string>? headers;

        public string GetHeaderValue(string headerKey)
        {
            return headers[headerKey];
        }

        protected Dictionary<string, string>? pathParameters;

        public string GetPathParameter(string key)
        {
            return pathParameters[key];
        }
        
        private static string StripApiPrefix(string apiPath)
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

        public static EntityBase FromRequestEvent(SessionEventArgs e)
        {
            string apiPath = StripApiPrefix(e.HttpClient.Request.RequestUri.AbsolutePath);

            foreach (var config in configs)
            {
                if (ApiPathMatch(config.Value.apiPath, apiPath) &&
                    config.Value.httpMethod == e.HttpClient.Request.Method)
                {
                    try
                    {
                        return (EntityBase) TypeDescriptor.CreateInstance(
                            provider: null,
                            objectType: config.Key,
                            argTypes: new[] {typeof(SessionEventArgs), typeof(Config)},
                            args: new object[] {e, config.Value})!;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        throw;
                    }
                }
            }

            return null; // error type
        }

        public EntityBase(SessionEventArgs e, Config config)
        { 
            sessionEventArgs = e;

            httpMethod = sessionEventArgs!.HttpClient.Request.Method;
            requestUri = sessionEventArgs!.HttpClient.Request.RequestUri;

            headers = new Dictionary<string, string>(e.HttpClient.Request.Headers.Count());
            foreach (var header in e.HttpClient.Request.Headers)
            {
                if (!headers.ContainsKey(header.Name))
                    headers.Add(header.Name, header.Value);
            }

            pathParameters = ExtractPathParameters(config.apiPath, StripApiPrefix(requestUri.AbsolutePath));
        }

        public static readonly Dictionary<Type, Config> configs = new Dictionary<Type, Config>
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