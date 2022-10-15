using System;
using System.Linq;
using Titanium.Web.Proxy.Http;
using Yuyuyui.PrivateServer;

namespace Yuyuyui.AccountTransfer
{
    public class EventItemsEntity : BaseEntity<EventItemsEntity>
    {
        public EventItemsEntity(
            Uri requestUri,
            string httpMethod,
            RouteConfig config)
            : base(requestUri, httpMethod, config)
        {
        }

        public override void ProcessRequest(byte[] requestBody, HeaderCollection requestHeaders,
            ref AccountTransferProxyCallbacks.PlayerSession playerSession)
        {
        }

        public override void ProcessResponse(byte[] responseBody, HeaderCollection responseHeaders,
            ref AccountTransferProxyCallbacks.PlayerSession playerSession)
        {
            var response = Deserialize<PrivateServer.EventItemsEntity.Response>(responseBody)!;

            playerSession.player!.items.eventItems = response.event_items
                .Select(p => new Item
                {
                    id = p.Value.id,
                    master_id = p.Value.master_id,
                    quantity = p.Value.quantity
                })
                .Select(p => { p.Save(); return p; })
                .ToDictionary(p => p.master_id, p => p.id);
            playerSession.player!.Save();
            
            Utils.LogTrace("Got event items.");
        }
    }
}