using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class SessionsEntity : BaseEntity<SessionsEntity>
    {
        public SessionsEntity(
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
            var requestObj = Deserialize<Request>(requestBody);
            Utils.Log($"Got connection from UUID {requestObj!.uuid}");
            
            PrivateServer.PlayerSession sessionDetail = PrivateServer.CreateSessionForPlayer(requestObj!.uuid, this);

            Response responseObj = new()
            {
                session_id = sessionDetail.sessionID,
                code = $"{sessionDetail.player.id.code}",
                unixtime = Utils.CurrentUnixTime(),
                gk_key = sessionDetail.sessionKey
            };

            sessionDetail.player.data.lastActive = responseObj.unixtime;
            sessionDetail.player.Save();
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders(sessionDetail.sessionID);
            
            return Task.CompletedTask;
        }

        public class Request
        {
            public string uuid { get; set; } = "";
        }

        public class Response
        {
            public string session_id { get; set; } = "";
            public string code { get; set; } = "";
            public long unixtime { get; set; }
            public string gk_key { get; set; } = "";
        }
    }
}