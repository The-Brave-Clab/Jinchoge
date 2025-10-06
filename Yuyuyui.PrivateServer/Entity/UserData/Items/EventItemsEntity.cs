using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class EventItemsEntity : BaseEntity<EventItemsEntity>
    {
        public EventItemsEntity(
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
            var player = GetPlayerFromCookies();

            Response responseObj;
            if (Config.Get().InGame.InfiniteItems)
            {
                List<EventItem> eventItems;
                await using (EventStoriesContext eventDb = new())
                {
                    eventItems = eventDb.EventItems.ToList();
                }

                responseObj = new()
                {
                    event_items = eventItems
                        .Select(i => new Item
                        {
                            id = i.Id,
                            master_id = i.Id,
                            quantity = 999
                        })
                        .ToDictionary(i => i.id, i => i)
                };
            }
            else
            {
                var eventItems = await player.items.eventItems
                    .Select(p => p.Value)
                    .Select(Item.Load)
                    .WhenAll();
                responseObj = new()
                {
                    event_items = eventItems
                        .Where(ei => ei.quantity > 0) // don't show consumed items
                        .ToDictionary(ei => ei.id, ei => ei)
                };
            }

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }

        public class Response
        {
            public IDictionary<long, Item> event_items { get; set; } = new Dictionary<long, Item>();
        }
    }
}