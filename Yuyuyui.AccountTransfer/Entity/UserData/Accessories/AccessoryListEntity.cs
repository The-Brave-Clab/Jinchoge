using System;
using Titanium.Web.Proxy.Http;
using Yuyuyui.PrivateServer;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.AccountTransfer
{
    public class AccessoryListEntity : BaseEntity<AccessoryListEntity>
    {
        public AccessoryListEntity(
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
            var response = Deserialize<PrivateServer.AccessoryListEntity.Response>(responseBody)!;

            foreach (var a in response.accessories)
            {
                Accessory accessory = new()
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
            
            Utils.LogTrace(string.Format(Resources.LOG_AT_GOT_ACCESSORIES, response.accessories.Count));
        }
    }
}