using Titanium.Web.Proxy.EventArguments;

namespace Yuyuyui.PrivateServer
{
    public class SessionsEntity : BaseEntity<SessionsEntity>
    {
        public SessionsEntity(SessionEventArgs e, Config config) : base(e, config)
        {
        }
    }
}