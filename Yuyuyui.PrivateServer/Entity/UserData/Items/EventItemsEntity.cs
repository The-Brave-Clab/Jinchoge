namespace Yuyuyui.PrivateServer
{
    public class EventItemsEntity : BaseEntity<EventItemsEntity>
    {
        public EventItemsEntity(
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

            if (!player.items.ContainsKey("event"))
            {
                player.items.Add("event", new List<long>());
            }

            Response responseObj = new()
            {
                event_items = player.items["event"].ToDictionary(c => c, c => Item.Load($"{c}"))
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IDictionary<long, Item> event_items { get; set; } = new Dictionary<long, Item>();
        }
    }
}