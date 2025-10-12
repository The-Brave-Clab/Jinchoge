using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer.AWS
{
    public class Function
    {
        private static ILambdaLogger? _logger;
        private static readonly Lazy<Task> _initTask;

        static Function()
        {
            _logger = null;
            _initTask = new Lazy<Task>(PrivateServer.Init);
            Utils.SetLogCallback(LogFunc);

            // Initialize providers
            PlayerDataProviderFactory.ActiveFactory = new DynamoDBPlayerDataProviderFactory();
            IPlayerProfileSessionProvider.ActiveProvider = new DynamoDBPlayerProfileSessionProvider();
            IDistributedLockProvider.ActiveProvider = new DynamoDBLockProvider();
            IMasterDataProvider.ActiveProvider = new LambdaMasterDataProvider();
            IInGameConfigProvider.ActiveProvider = new ConfigPlayerInGameConfigProvider();
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(
            APIGatewayHttpApiV2ProxyRequest apiRequest,
            ILambdaContext context)
        {
            _logger = context.Logger;

            // Set Lambda request context for distributed lock provider
            // This enables Lambda-context-wise reentrant locks and better traceability
            DynamoDBLockProvider.CurrentLambdaRequestId = context.AwsRequestId;

            await _initTask.Value;

            try
            {
                // Extract body (handle base64 encoding)
                byte[] requestBodyBytes = ExtractRequestBody(apiRequest);

                // Convert to EventArgs
                var eventArgs = new EventArgs
                {
                    requestUri = new Uri($"https://{apiRequest.RequestContext.DomainName}{apiRequest.RawPath}"),
                    requestMethod = apiRequest.RequestContext.Http.Method,
                    header = ConvertHeaders(apiRequest.Headers, apiRequest.Cookies),
                    requestBody = requestBodyBytes // Pass raw bytes (encrypted or not)
                };

                // Route to entity
                var entity = EntityBase.FromEventArgs(eventArgs);
                entity = await EntityBase.Process(entity);

                // Return response
                if (entity is RequestErrorEntity errorEntity)
                {
                    return new APIGatewayHttpApiV2ProxyResponse
                    {
                        StatusCode = errorEntity.StatusCode,
                        Body = Encoding.UTF8.GetString(errorEntity.ResponseBody),
                        Headers = errorEntity.ResponseHeaders,
                        IsBase64Encoded = false
                    };
                }

                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 200,
                    Body = Encoding.UTF8.GetString(entity.ResponseBody),
                    Headers = entity.ResponseHeaders,
                    IsBase64Encoded = false
                };
            }
            catch (Exception ex)
            {
                // Handle unknown errors
                Utils.LogError($"Unhandled exception in Lambda function: \n{ex.Message}\n{ex.StackTrace}");
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 500,
                    Body = $"Internal server error: {ex.Message}",
                    Headers = new Dictionary<string, string>(),
                    IsBase64Encoded = false
                };
            }
            finally
            {
                // Clear Lambda request context to prevent issues with container reuse
                // CRITICAL: Must be called after every request
                DynamoDBLockProvider.ClearRequestContext();
                _logger = null;
            }
        }

        private byte[] ExtractRequestBody(APIGatewayHttpApiV2ProxyRequest request)
        {
            if (string.IsNullOrEmpty(request.Body))
                return [];

            return request.IsBase64Encoded
                ? Convert.FromBase64String(request.Body)
                : Encoding.UTF8.GetBytes(request.Body);
        }

        private Dictionary<string, string> ConvertHeaders(
            IDictionary<string, string> headers,
            string[]? cookies)
        {
            var result = new Dictionary<string, string>(headers);
            
            // Add cookies as Cookie header (EntityBase expects this)
            if (cookies != null && cookies.Length > 0)
            {
                result["Cookie"] = string.Join("; ", cookies);
            }
            
            return result;
        }

        private static void LogFunc(object o, Utils.LogType t)
        {
            switch (t)
            {
                case Utils.LogType.Trace:
                    _logger?.LogTrace(o.ToString());
                    break;
                case Utils.LogType.Info:
                    _logger?.LogInformation(o.ToString());
                    break;
                case Utils.LogType.Warning:
                    _logger?.LogWarning(o.ToString());
                    break;
                case Utils.LogType.Error:
                    _logger?.LogError(o.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(t), t, null);
            }
        }
    }
}