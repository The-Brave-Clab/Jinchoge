using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class CheckBattleTokensEntity : BaseEntity<CheckBattleTokensEntity>
    {
        public CheckBattleTokensEntity(
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
            Utils.LogWarning("Stub API! Returns only false for now.");

            Response responseObj = new()
            {
                is_token = false
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public bool is_token { get; set; } = false;
        }
    }
}