using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class MissionADVUpdateEntity : BaseEntity<MissionADVUpdateEntity>
    {
        public MissionADVUpdateEntity(Uri requestUri, string httpMethod, Dictionary<string, string> requestHeaders, byte[] requestBody, RouteConfig config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config) { }

        protected override Task ProcessRequest()
        {
            long missionId = long.Parse(GetPathParameter("mission_adventure_id"));
            responseBody = Serialize(new Response { mission_adventure = new MissionAdventure { id = missionId, received = true } });
            SetBasicResponseHeaders();
            return Task.CompletedTask;
        }

        public class Response
        {
            public MissionAdventure mission_adventure { get; set; } = new();
        }

        public class MissionAdventure
        {
            public long id { get; set; }
            public bool received { get; set; }
        }
    }
}
