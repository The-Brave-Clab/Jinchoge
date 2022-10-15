using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace Yuyuyui.PrivateServer
{
    public interface IProxyCallbacks
    {
        Task OnRequest(object sender, SessionEventArgs e);
        Task OnResponse(object sender, SessionEventArgs e);
        Task OnCertificateValidation(object sender, CertificateValidationEventArgs e);
        Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e);
        Task OnBeforeTunnelConnect(object sender, TunnelConnectSessionEventArgs e);
    }
}