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
            var playerId = await GetPlayerIdFromCookies();

            bool infiniteItems;
            IDictionary<long, long> playerEvolutionItems;
            await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
            {
                var player = await PlayerProfile.Load(playerId.code);
                infiniteItems = await PrivateServer.ResourceProvider.inGameConfigProvider.GetInfiniteItems(player);
                playerEvolutionItems = player.items.evolution;
            }

            Response responseObj;
            if (infiniteItems)
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
                var evolutionItems = await Item.LoadMany(
                    playerEvolutionItems
                        .Select(p => p.Value));
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