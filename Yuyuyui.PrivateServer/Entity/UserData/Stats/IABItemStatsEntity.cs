namespace Yuyuyui.PrivateServer
{
    public class IABItemStatsEntity : BaseEntity<IABItemStatsEntity>
    {
        public IABItemStatsEntity(
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

            Utils.LogWarning("Fixed number of 1,000,000 paid blessings");

            Response responseObj = new()
            {
                paid_point = player.data.paidBlessing,
                free_point = 0
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public int paid_point { get; set; }
            public int free_point { get; set; }
        }
    }
}