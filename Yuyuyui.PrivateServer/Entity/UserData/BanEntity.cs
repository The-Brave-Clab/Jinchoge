using System.Text;

namespace Yuyuyui.PrivateServer
{
    public class BanEntity : BaseEntity<BanEntity>
    {
        public BanEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            PlayerProfile player = GetPlayerFromCookies();

            player.BanAccount();
            
            responseBody = Encoding.UTF8.GetBytes("{}");
            SetBasicResponseHeaders();
            
            return Task.CompletedTask;
        }
    }
}