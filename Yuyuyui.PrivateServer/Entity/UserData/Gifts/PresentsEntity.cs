namespace Yuyuyui.PrivateServer
{
    public class PresentsEntity : BaseEntity<PresentsEntity>
    {
        public PresentsEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();

            Response responseObj = new()
            {
                gifts = player.receivedGifts.Select(Gift.Load).ToList()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IList<Gift> gifts { get; set; } = new List<Gift>();
        }
    }
}