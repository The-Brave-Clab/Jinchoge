using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class AutoClearTicketsEntity : BaseEntity<AutoClearTicketsEntity>
    {
        public AutoClearTicketsEntity(
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
                await using ItemsContext itemsDb = new();
                responseObj = new()
                {
                    tickets = itemsDb.AutoClearTickets
                        .Select(t => new Item
                        {
                            id = t.Id,
                            master_id = t.Id,
                            quantity = 999
                        })
                        .ToList()
                };
            }
            else
            {
                var autoClearTickets = await player.items.autoClearTickets
                    .Select(p => p.Value)
                    .Select(Item.Load)
                    .WhenAll();
                responseObj = new()
                {
                    tickets = autoClearTickets
                        .Where(t => t.quantity > 0)
                        .ToList()
                };
            }

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }

        public class Response
        {
            public IList<Item> tickets { get; set; } = new List<Item>();
        }
    }
}