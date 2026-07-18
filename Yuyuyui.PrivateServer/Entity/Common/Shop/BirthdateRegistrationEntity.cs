using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class BirthdateRegistrationEntity : BaseEntity<BirthdateRegistrationEntity>
    {
        public BirthdateRegistrationEntity(
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
            player.data.birthdateRegistered = true;
            player.Save();

            responseBody = Serialize(new Response());
            SetBasicResponseHeaders();
            return Task.CompletedTask;
        }

        public class Response
        {
            public bool registered { get; set; } = true;
        }
    }
}
