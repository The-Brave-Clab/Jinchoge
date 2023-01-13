using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class StaminaItemsEntity : BaseEntity<StaminaItemsEntity>
    {
        public StaminaItemsEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            RouteConfig config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();

            Response responseObj;
            if (Config.Get().InGame.InfiniteItems)
            {
                using var itemsDb = new ItemsContext();
                responseObj = new()
                {
                    stamina_items = itemsDb.StaminaItems
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
                responseObj = new()
                {
                    stamina_items = player.items.stamina
                        .Select(p => p.Value)
                        .Select(Item.Load)
                        .Where(si => si.quantity > 0)
                        .ToDictionary(si => si.id, si => si)
                };
            }

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