using System.Net;

namespace Yuyuyui.PrivateServer
{
    public class EnhancementItemsEntity : BaseEntity<EnhancementItemsEntity>
    {
        public EnhancementItemsEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config,
            IPEndPoint localEndPoint)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config, localEndPoint)
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
                enhancement_items = player.items["enhancement"].ToDictionary(c => c, c => Item.Load($"{c}"))
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