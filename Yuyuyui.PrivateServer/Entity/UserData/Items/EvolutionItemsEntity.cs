using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class EvolutionItemsEntity : BaseEntity<EvolutionItemsEntity>
    {
        public EvolutionItemsEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            RouteConfig config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override async Task ProcessRequest()
        {
            var player = await GetPlayerFromCookies();

            Response responseObj;
            if (Config.Get().InGame.InfiniteItems)
            {
                List<EvolutionItem> evolutionItems;
                await using (ItemsContext itemsDb = new())
                {
                    evolutionItems = itemsDb.EvolutionItems.ToList();
                }

                responseObj = new()
                {
                    evolution_items = evolutionItems
                        .Select(t => new Item
                        {
                            id = t.Id,
                            master_id = t.Id,
                            quantity = 999
                        })
                        .ToDictionary(i => i.id, i => i)
                };
            }
            else
            {
                var evolutionItems = await player.items.evolution
                    .Select(p => p.Value)
                    .Select(Item.Load)
                    .WhenAll();
                responseObj = new()
                {
                    evolution_items = evolutionItems
                        .Where(ei => ei.quantity > 0) // don't show consumed items
                        .ToDictionary(ei => ei.id, ei => ei)
                };
            }

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }

        public class Response
        {
            public IDictionary<long, Item> evolution_items { get; set; } = new Dictionary<long, Item>();
        }
    }
}