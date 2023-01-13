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

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();

            Response responseObj;
            if (Config.Get().InGame.InfiniteItems)
            {
                using var itemsDb = new ItemsContext();
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
                responseObj = new()
                {
                    tickets = player.items.autoClearTickets
                        .Select(p => p.Value)
                        .Select(Item.Load)
                        .Where(t => t.quantity > 0)
                        .ToList()
                };
            }

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IList<Item> tickets { get; set; } = new List<Item>();
        }
    }
}