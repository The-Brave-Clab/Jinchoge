using Titanium.Web.Proxy.EventArguments;
using Yuyuyui.PrivateServer;

namespace Yuyuyui.AccountTransfer
{
    public class AccountTransferProxyCallbacks : IProxyCallbacks
    {
        public Task OnRequest(object sender, SessionEventArgs e)
        {
            throw new NotImplementedException();
        }

        public Task OnResponse(object sender, SessionEventArgs e)
        {
            throw new NotImplementedException();
        }

        public Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            throw new NotImplementedException();
        }

        public Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e)
        {
            throw new NotImplementedException();
        }

        public Task OnBeforeTunnelConnect(object sender, TunnelConnectSessionEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
