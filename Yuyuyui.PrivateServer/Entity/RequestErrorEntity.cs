using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class RequestErrorEntity : BaseEntity<RequestErrorEntity>
    {
        private Response responseObj;

        private string? logMessage = null;

        public int StatusCode => responseObj.error.status;

        public RequestErrorEntity(
            string code,
            string message,
            Uri requestedUri,
            string httpMethod,
            RouteConfig requestConfig,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            string? logMessage = null
        )
            : base(requestedUri, httpMethod, requestHeaders, requestBody, requestConfig)
        {
            responseObj = new Response
            {
                error = new ErrorBody
                {
                    code = code,
                    message = message,
                    status = 404
                }
            };
            this.logMessage = logMessage;

            if (code == "U0401")
                responseObj.error.status = 401;
            else if (code == "A4101")
                responseObj.error.status = 422;
        }

        protected override Task ProcessRequest()
        {
            // Error Codes:
            // S1000: App Update
            // S2000: Maintenance, shows message
            // A1321: Network error, small window with message, with prefix
            // A0120: Input Error
            // A4101: HTTP 422, Login Failed, ログインに失敗しました。 このメッセージが繰り返し表示される場合、 お問い合わせフォームよりご連絡ください。
            // U0401: HTTP 401, Unauthorized Error
            string messageBody = string.IsNullOrEmpty(logMessage) ? responseObj.error.message : logMessage!;
            
            Utils.LogError($"<APIError> {responseObj.error.code}: {messageBody}");
            
            if (HasRequestBody())
            {
                string requestBodyStr = Encoding.UTF8.GetString(requestBody);
                Utils.LogError($"Request Body:\n{requestBodyStr}");
            }

            Utils.LogError("Request Headers:");
            foreach (var header in requestHeaders)
            {
                Utils.LogError($"\t{header.Key}: {header.Value}");
            }

            responseBody = Serialize(responseObj);

            return Task.CompletedTask;
        }

        private class Response
        {
            public ErrorBody error { get; set; } = new();
        }

        private class ErrorBody
        {
            public string code { get; set; } = "";
            public string message { get; set; } = "";
            public int status { get; set; }
        }
    }

    public class APIErrorException : Exception
    {
        public string errorCode;
        public object body;

        public APIErrorException(string errorCode, object body)
            : base($"<APIError> {errorCode}: {body}")
        {
            this.errorCode = errorCode;
            this.body = body;
        }

        public override string ToString()
        {
            return $"<APIError> {errorCode}: {body}";
        }
    }
}