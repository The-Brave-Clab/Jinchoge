using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class BuffsEntity : BaseEntity<BuffsEntity>
    {
        public BuffsEntity(
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
            var player = GetPlayerFromCookies();
            
            Utils.LogError("Not documented! Returns nothing for now.");

            Response responseObj = new()
            {
                buffs = new List<int>()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IList<int> buffs { get; set; } = new List<int>(); // the type of content is unknown!
        }
    }
}