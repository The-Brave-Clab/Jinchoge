﻿namespace Yuyuyui.PrivateServer
{
    public class EvolutionItemsEntity : BaseEntity<EvolutionItemsEntity>
    {
        public EvolutionItemsEntity(
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
                evolution_items = player.items.evolution
                    .Select(p => p.Value)
                    .Select(Item.Load)
                    .Where(ei => ei.quantity > 0) // don't show consumed items
                    .ToDictionary(ei => ei.id, ei => ei)
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IDictionary<long, Item> evolution_items { get; set; } = new Dictionary<long, Item>();
        }
    }
}