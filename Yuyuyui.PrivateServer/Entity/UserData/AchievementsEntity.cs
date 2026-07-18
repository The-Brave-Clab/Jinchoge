using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class AchievementsEntity : BaseEntity<AchievementsEntity>
    {
        public AchievementsEntity(
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
            responseBody = Serialize(new Response
            {
                accepted = true,
                completed = true,
                achievements = new List<Achievement>()
            });
            SetBasicResponseHeaders();
            return Task.CompletedTask;
        }

        public class Response
        {
            public bool accepted { get; set; }
            public bool completed { get; set; }
            public IList<Achievement> achievements { get; set; } = new List<Achievement>();
        }

        public class Achievement
        {
            public string id { get; set; } = "";
            public int progress { get; set; }
            public bool completed { get; set; }
        }
    }
}
