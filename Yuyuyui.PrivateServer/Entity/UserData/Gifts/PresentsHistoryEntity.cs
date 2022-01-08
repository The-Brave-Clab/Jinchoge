namespace Yuyuyui.PrivateServer
{
    public class PresentsHistoryEntity : BaseEntity<PresentsHistoryEntity>
    {
        public PresentsHistoryEntity(
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

            PresentsEntity.Response responseObj = new()
            {
                gifts = player.receivedGifts.Select(Gift.Load).ToList()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }
    }
}