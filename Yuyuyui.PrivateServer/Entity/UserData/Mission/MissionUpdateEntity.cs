using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class MissionUpdateEntity : BaseEntity<MissionUpdateEntity>
    {
        public MissionUpdateEntity(Uri requestUri, string httpMethod, Dictionary<string, string> requestHeaders, byte[] requestBody, RouteConfig config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config) { }

        protected override Task ProcessRequest()
        {
            long missionId = long.Parse(GetPathParameter("mission_id"));
            responseBody = Serialize(new Response { mission = new Mission { id = missionId, received = true } });
            SetBasicResponseHeaders();
            return Task.CompletedTask;
        }

        public class Response
        {
            public Mission mission { get; set; } = new();
        }

        public class Mission
        {
            public long id { get; set; }
            public bool received { get; set; }
        }
    }
}
