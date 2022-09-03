using Titanium.Web.Proxy.Http;
using Yuyuyui.PrivateServer;

namespace Yuyuyui.AccountTransfer
{
    public class EnhancementItemsEntity : BaseEntity<EnhancementItemsEntity>
    {
        public EnhancementItemsEntity(
            Uri requestUri,
            string httpMethod,
            Config config)
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
            var response = Deserialize<PrivateServer.EnhancementItemsEntity.Response>(responseBody)!;

            playerSession.player!.items.enhancement = response.enhancement_items
                .Select(ei => new Item
                {
                    id = ei.id,
                    master_id = ei.master_id,
                    quantity = ei.quantity
                })
                .Select(p => { p.Save(); return p; })
                .ToDictionary(p => p.master_id, p => p.id);
            playerSession.player!.Save();
            
            Utils.LogTrace("Got enhancement items.");
            
            TransferProgress.Completed(TransferProgress.TaskType.EnhancementItems);
        }
    }
}