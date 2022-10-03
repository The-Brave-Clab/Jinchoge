using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Utils = Yuyuyui.PrivateServer.Utils;

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
        public readonly TransferProgress.TaskType TransferTask;
        
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

            TransferTask = config.transferTask;

            pathParameters = ExtractPathParameters(config.apiPath, StripApiPrefix(requestUri.AbsolutePath))!;
        }

        private static readonly Dictionary<Type, Config> configs = new()
        {
            {
                typeof(SessionsEntity),
                new Config(TransferProgress.TaskType.Id, "/sessions", "POST")
            },
            {
                typeof(HeaderEntity),
                new Config(TransferProgress.TaskType.Header, "/my/header", "GET")
            },
            {
                typeof(UserInfoEntity),
                new Config(TransferProgress.TaskType.Profile, "/users/{user_id}", "GET")
            },
            {
                typeof(AccessoryListEntity),
                new Config(TransferProgress.TaskType.Accessories, "/my/accessories", "GET")
            },
            {
                typeof(CardsEntity),
                new Config(TransferProgress.TaskType.Cards, "/my/cards", "GET")
            },
            {
                typeof(DeckEntity),
                new Config(TransferProgress.TaskType.Decks, "/my/decks", "GET")
            },
            {
                typeof(EnhancementItemsEntity),
                new Config(TransferProgress.TaskType.EnhancementItems, "/my/enhancement_items", "GET")
            },
            {
                typeof(EventItemsEntity),
                new Config(TransferProgress.TaskType.EventItems, "/my/event_items", "GET")
            },
            {
                typeof(EvolutionItemsEntity),
                new Config(TransferProgress.TaskType.EvolutionItems, "/my/evolution_items", "GET")
            },
            {
                typeof(StaminaItemsEntity),
                new Config(TransferProgress.TaskType.StaminaItems, "/my/stamina_items", "GET")
            },
            {
                typeof(TitleItemsEntity),
                new Config(TransferProgress.TaskType.TitleItems, "/my/title_items", "GET")
            },
            {
                typeof(CharacterFamiliarityEntity),
                new Config(TransferProgress.TaskType.CharacterFamiliarities, "/my/character_familiarities", "GET")
            },
        };
    }

    public struct Config
    {
        public Config(TransferProgress.TaskType transferTask, string apiPath, params string[] httpMethods)
        {
            this.transferTask = transferTask;
            this.apiPath = apiPath;
            this.httpMethods = httpMethods;
        }

        public readonly TransferProgress.TaskType transferTask;
        public readonly string apiPath;
        public readonly string[] httpMethods;
    }
}