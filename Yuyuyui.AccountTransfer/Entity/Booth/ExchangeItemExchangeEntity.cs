using Titanium.Web.Proxy.Http;

namespace Yuyuyui.AccountTransfer.Entity.Booth;

public class ExchangeItemExchangeEntity : BaseEntity<ExchangeItemExchangeEntity>
{
    public ExchangeItemExchangeEntity(
        Uri requestUri,
        string httpMethod,
        Config config)
        : base(requestUri, httpMethod, config)
    {
    }

    public override void ProcessRequest(byte[] requestBody, HeaderCollection requestHeaders,
        ref AccountTransferProxyCallbacks.PlayerSession playerSession)
    {
        // string requestStr = System.Text.Encoding.Default.GetString(requestBody);
        // Utils.LogTrace("Exchange");
        // Utils.LogTrace(requestStr);
    }
    
    public override void ProcessResponse(byte[] responseBody, HeaderCollection responseHeaders,
        ref AccountTransferProxyCallbacks.PlayerSession playerSession)
    {
        // string responseStr = System.Text.Encoding.Default.GetString(responseBody);
        // Utils.LogTrace($"Got Exchange Booth Entity");
    }
}