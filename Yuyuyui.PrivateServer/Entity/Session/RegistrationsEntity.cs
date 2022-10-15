using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class RegistrationsEntity : BaseEntity<RegistrationsEntity>
    {
        public RegistrationsEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            RouteConfig config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
            
        }
        
        protected override Task ProcessRequest()
        {
            var requestObj = Deserialize<SessionsEntity.Request>(requestBody);
            PrivateServer.PlayerSession sessionDetail = PrivateServer.CreateSessionForPlayer(requestObj!.uuid, this);

            SessionsEntity.Response responseObj = new()
            {
                session_id = sessionDetail.sessionID,
                code = $"{sessionDetail.player.id.code}",
                unixtime = Utils.CurrentUnixTime(),
                gk_key = sessionDetail.sessionKey
            };
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
            
            return Task.CompletedTask;
        }
    }
}
