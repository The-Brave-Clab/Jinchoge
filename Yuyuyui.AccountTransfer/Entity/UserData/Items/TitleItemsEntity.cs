using Titanium.Web.Proxy.Http;
using Yuyuyui.PrivateServer;

namespace Yuyuyui.AccountTransfer
{
    public class TitleItemsEntity : BaseEntity<TitleItemsEntity>
    {
        public TitleItemsEntity(
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
            var response = Deserialize<PrivateServer.TitleItemsEntity.GetResponse>(responseBody)!;

            playerSession.player!.items.titleItems = response.title_items
                .Select(p => p.Value)
                .Where(ti => ti.id != null)
                .Select(ti => ti.master_id)
                .ToList();
            playerSession.player!.Save();
            
            Utils.LogTrace($"Got titles, {playerSession.player!.items.titleItems.Count} in total.");
        }
    }
}