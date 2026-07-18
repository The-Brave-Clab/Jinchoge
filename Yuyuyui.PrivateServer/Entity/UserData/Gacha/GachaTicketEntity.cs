using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class GachaTicketEntity : BaseEntity<GachaTicketEntity>
    {
        public GachaTicketEntity(
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
            Response responseObj = new();

            using var gachasDb = new GachasContext();

            if (Config.Get().InGame.InfiniteItems)
            {
                responseObj.gacha_tickets = gachasDb.GachaTickets
                    .ToList()
                    .Select(t => ToTicket(t, 999))
                    .ToList();
            }
            else
            {
                var player = GetPlayerFromCookies();
                responseObj.gacha_tickets = player.items.gachaTickets
                    .Select(p => new
                    {
                        Master = gachasDb.GachaTickets.FirstOrDefault(t => t.Id == p.Key),
                        UserItem = Item.Exists(p.Value) ? Item.Load(p.Value) : null
                    })
                    .Where(t => t.Master != null && t.UserItem != null && t.UserItem.quantity > 0)
                    .Select(t => ToTicket(t.Master!, t.UserItem!.quantity))
                    .ToList();
            }

            responseObj.tickets = responseObj.gacha_tickets;

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        private static Ticket ToTicket(DataModel.GachaTicket ticket, int quantity)
        {
            return new Ticket
            {
                id = ticket.Id,
                master_id = ticket.Id,
                gacha_id = ticket.GachaId,
                gacha_kind = ticket.GachaKind,
                consumption_resource_id = ticket.ConsumptionResourceId,
                image_id = ticket.ImageId,
                name = ticket.Name,
                quantity = quantity
            };
        }

        public class Response
        {
            public IList<Ticket> gacha_tickets { get; set; } = new List<Ticket>();
            public IList<Ticket> tickets { get; set; } = new List<Ticket>();
        }

        public class Ticket
        {
            public long id { get; set; }
            public long master_id { get; set; }
            public long gacha_id { get; set; }
            public int gacha_kind { get; set; }
            public int consumption_resource_id { get; set; }
            public long image_id { get; set; }
            public string name { get; set; } = "";
            public int quantity { get; set; }
        }
    }
}
