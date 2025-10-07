using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class TutorialProgressEntity : BaseEntity<TutorialProgressEntity>
    {
        public TutorialProgressEntity(
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
            var player = await GetPlayerFromCookies();
            if (requestBody.Length > 0)
            {
                Request requestObj = Deserialize<Request>(requestBody)!;
                player.data.tutorialProgress = requestObj.progress;
                await player.Save();
            }

            Response responseObj = new()
            {
                progress = player.data.tutorialProgress,
                tutee = player.data.tutorialProgress != 1000
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }

        public class Request
        {
            public int progress { get; set; }
        }

        public class Response
        {
            public bool tutee { get; set; }
            public int progress { get; set; }
        }
    }
}