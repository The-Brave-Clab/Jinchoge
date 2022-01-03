namespace Yuyuyui.PrivateServer
{
    public class NotImplementedErrorEntity : BaseEntity<NotImplementedErrorEntity>
    {
        public NotImplementedErrorEntity(
            Uri requestUri,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config)
            : base(requestUri, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            // Error Codes:
            // S1000: App Update
            // S2000: Maintenance, shows message
            // A1321: Network error, small window with message, with prefix
            // A0120: Input Error

            string errorMessage = $"API {StripApiPrefix(RequestUri.AbsolutePath)} has not been implemented yet.";
            Console.WriteLine(errorMessage);
            Response responseObj = new Response
            {
                error = new ErrorBody
                {
                    code = "S2000",
                    message = errorMessage,
                    status = 0
                }
            };

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