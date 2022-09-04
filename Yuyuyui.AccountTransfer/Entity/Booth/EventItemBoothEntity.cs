using Titanium.Web.Proxy.Http;
using Yuyuyui.PrivateServer;
using Yuyuyui.PrivateServer.Entity;

namespace Yuyuyui.AccountTransfer.Entity.Booth;

public class EventItemBoothEntity : BaseEntity<EventItemBoothEntity>
{
    public EventItemBoothEntity(
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
        // string responseStr = System.Text.Encoding.Default.GetString(responseBody);
        // var response = Deserialize<BoothEventResponse>(responseBody)!;
        Utils.LogTrace($"Got Event Item Booth Entity");
    }
}