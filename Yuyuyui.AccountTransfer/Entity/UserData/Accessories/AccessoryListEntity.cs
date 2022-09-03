using Titanium.Web.Proxy.Http;
using Yuyuyui.PrivateServer;

namespace Yuyuyui.AccountTransfer
{
    public class AccessoryListEntity : BaseEntity<AccessoryListEntity>
    {
        public AccessoryListEntity(
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
            var response = Deserialize<PrivateServer.AccessoryListEntity.Response>(responseBody)!;

            foreach (var a in response.accessories)
            {
                Accessory accessory = new Accessory()
                {
                    id = a.Value.id,
                    master_id = a.Value.master_id,
                    level = a.Value.level,
                    quantity = a.Value.quantity
                };
                accessory.Save();

                playerSession.player!.accessories[accessory.master_id] = accessory.id;
            }
            playerSession.player!.Save();
            
            Utils.LogTrace($"Got accessories, {response.accessories.Count} in total.");
            
            TransferProgress.Completed(TransferProgress.TaskType.Accessories);
        }
    }
}