﻿namespace Yuyuyui.PrivateServer
{
    public class StaminaItemsEntity : BaseEntity<StaminaItemsEntity>
    {
        public StaminaItemsEntity(
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

            if (!player.items.ContainsKey("stamina"))
            {
                player.items.Add("stamina", new List<long>());
            }

            Response responseObj = new()
            {
                stamina_items = player.items["stamina"].ToDictionary(c => c, Item.Load)
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IDictionary<long, Item> stamina_items { get; set; } = new Dictionary<long, Item>();
        }
    }
}