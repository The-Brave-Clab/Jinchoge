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

        protected override async Task ProcessRequest()
        {
            var player = await GetPlayerFromCookies();

            Response responseObj;
            if (await IInGameConfigProvider.ActiveProvider!.GetInfiniteItems(player))
            {
                List<StaminaItem> staminaItems;
                await using (ItemsContext itemsDb = new())
                {
                    staminaItems = itemsDb.StaminaItems.ToList();
                }

                responseObj = new()
                {
                    stamina_items = staminaItems
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
                var staminaItems = await Item.LoadMany(
                    player.items.stamina
                        .Select(p => p.Value));
                responseObj = new()
                {
                    stamina_items = staminaItems
                        .Where(si => si.quantity > 0)
                        .ToDictionary(si => si.id, si => si)
                };
            }

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }

        public class Response
        {
            public IDictionary<long, Item> stamina_items { get; set; } = new Dictionary<long, Item>();
        }
    }
}