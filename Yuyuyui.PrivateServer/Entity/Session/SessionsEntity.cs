using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.Localization;

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

        protected override async Task ProcessRequest()
        {
            var requestObj = Deserialize<Request>(requestBody);
            Utils.Log(string.Format(Resources.LOG_PS_GOT_CONNECTION, requestObj!.uuid));
            
            IPlayerProfileSessionProvider.PlayerSession sessionDetail =
                await PrivateServer.CreateSessionForPlayer(requestObj!.uuid, this);

            Response responseObj = new()
            {
                session_id = sessionDetail.session.id,
                code = sessionDetail.playerId.code,
                unixtime = Utils.CurrentUnixTime(),
                gk_key = sessionDetail.session.key
            };

            await using (await IDistributedLockProvider.ActiveProvider!.AcquirePlayerProfileLock(sessionDetail.playerId.code))
            {
                var player = await PlayerProfile.Load(sessionDetail.playerId.code);
                player.data.lastActive = responseObj.unixtime;
                await player.Save();
            }
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders(sessionDetail.session.id);
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