namespace Yuyuyui.PrivateServer
{
    public class AutoClearTicketsEntity : BaseEntity<AutoClearTicketsEntity>
    {
        public AutoClearTicketsEntity(
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
                tickets = player.items.autoClearTickets
                    .Select(p => p.Value)
                    .Select(Item.Load).ToList()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IList<Item> tickets { get; set; } = new List<Item>();
        }
    }
}