namespace Yuyuyui.PrivateServer
{
    public class RequestErrorEntity : BaseEntity<RequestErrorEntity>
    {
        private Response responseObj;
        public RequestErrorEntity(string code, string message, Uri requestedUri, Config requestConfig)
            : base(requestedUri, new Dictionary<string, string>(), Array.Empty<byte>(), requestConfig)
        {
            responseObj = new Response
            {
                error = new ErrorBody
                {
                    code = code,
                    message = message,
                    status = 0
                }
            };
        }

        protected override Task ProcessRequest()
        {
            // Error Codes:
            // S1000: App Update
            // S2000: Maintenance, shows message
            // A1321: Network error, small window with message, with prefix
            // A0120: Input Error
            Console.WriteLine(responseObj.error.message);

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
}