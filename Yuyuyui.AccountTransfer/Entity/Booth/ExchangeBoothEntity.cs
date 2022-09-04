using Titanium.Web.Proxy.Http;
using Yuyuyui.PrivateServer;
using Yuyuyui.PrivateServer.Responses;

namespace Yuyuyui.AccountTransfer.Entity.Booth;

public class ExchangeBoothEntity : BaseEntity<ExchangeBoothEntity>
{
    public ExchangeBoothEntity(
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
        // var response = Deserialize<BoothResponse>(responseBody)!;
        Utils.LogTrace($"Got Exchange Booth Entity");
    }
}