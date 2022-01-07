using System.Net;

namespace Yuyuyui.PrivateServer
{
    public class ScenarioResourceVersionEntity : GameResourceVersionEntity
    {
        public ScenarioResourceVersionEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config,
            IPEndPoint localEndPoint)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config, localEndPoint)
        {
        }
    }
}