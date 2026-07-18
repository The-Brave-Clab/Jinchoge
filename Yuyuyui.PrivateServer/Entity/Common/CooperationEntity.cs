using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class CooperationEntity : BaseEntity<CooperationEntity>
    {
        public CooperationEntity(
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
            PlayerProfile? player = null;
            try
            {
                player = GetPlayerFromCookies();
            }
            catch (APIErrorException)
            {
                // Some cooperation confirmation calls can be made before a full session exists.
            }

            Response responseObj = new()
            {
                accepted = true,
                completed = true,
                cooperation = new()
                {
                    id = player?.id.code ?? "",
                    user_id = player?.id.code ?? "",
                    provider = "google_play",
                    platform = "google_play",
                    linked = true,
                    cooperated = true
                }
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
            return Task.CompletedTask;
        }

        public class Response
        {
            public bool accepted { get; set; }
            public bool completed { get; set; }
            public Cooperation cooperation { get; set; } = new();
        }

        public class Cooperation
        {
            public string id { get; set; } = "";
            public string user_id { get; set; } = "";
            public string provider { get; set; } = "google_play";
            public string platform { get; set; } = "google_play";
            public bool linked { get; set; }
            public bool cooperated { get; set; }
        }
    }
}
