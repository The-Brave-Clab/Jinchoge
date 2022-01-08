namespace Yuyuyui.PrivateServer
{
    public class EnhancementItemsEntity : BaseEntity<EnhancementItemsEntity>
    {
        public EnhancementItemsEntity(
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

            if (!player.items.ContainsKey("enhancement"))
            {
                player.items.Add("enhancement", new List<long>());
            }

            Response responseObj = new()
            {
                enhancement_items = player.items["enhancement"].ToDictionary(c => c, Item.Load)
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IDictionary<long, Item> enhancement_items { get; set; } = new Dictionary<long, Item>();
        }
    }
}