using Titanium.Web.Proxy.Http;
using Yuyuyui.PrivateServer;

namespace Yuyuyui.AccountTransfer
{
    public class AutoClearTicketsEntity : BaseEntity<AutoClearTicketsEntity>
    {
        public AutoClearTicketsEntity(
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
            var response = Deserialize<PrivateServer.AutoClearTicketsEntity.Response>(responseBody)!;

            playerSession.player!.items.autoClearTickets = response.tickets
                .Select(p => { p.Save(); return p; })
                .ToDictionary(p => p.master_id, p => p.id);
            playerSession.player!.Save();
            
            Utils.LogTrace("Got auto clear tickets.");
            
            TransferProgress.Completed(TransferProgress.TaskType.AutoClearTickets);
        }
    }
}