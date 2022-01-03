using System.Text;

namespace Yuyuyui.PrivateServer
{
    public class RegistrationsEntity : BaseEntity<RegistrationsEntity>
    {
        public RegistrationsEntity(
            Uri requestUri,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config)
            : base(requestUri, requestHeaders, requestBody, config)
        {
            
        }
        
        protected override Task ProcessRequest()
        {
            Console.WriteLine(Encoding.UTF8.GetString(requestBody));
            var requestObj = Deserialize<SessionsEntity.Request>(requestBody);
            PrivateServer.PlayerSession sessionDetail = PrivateServer.CreateSessionForPlayer(requestObj!.uuid);

            SessionsEntity.Response responseObj = new()
            {
                session_id = sessionDetail.sessionID,
                code = sessionDetail.player.code,
                unixtime = Utils.CurrentUnixTime(),
                gk_key = sessionDetail.sessionKey
            };
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders(sessionDetail.sessionID);
            
            return Task.CompletedTask;
        }
    }
}
