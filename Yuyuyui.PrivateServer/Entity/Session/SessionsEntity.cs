﻿using System.Text;

namespace Yuyuyui.PrivateServer
{
    public class SessionsEntity : BaseEntity<SessionsEntity>
    {
        public SessionsEntity(
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
            var requestObj = Deserialize<Request>(requestBody);
            PrivateServer.PlayerSession sessionDetail = PrivateServer.CreateSessionForPlayer(requestObj!.uuid);

            Response responseObj = new()
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