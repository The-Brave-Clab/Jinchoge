using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class ChapterEntity : BaseEntity<ChapterEntity>
    {
        public ChapterEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            Response responseObj = GetChapters();

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        protected virtual Response GetChapters()
        {
            var player = GetPlayerFromCookies();
            
            Utils.LogWarning("Locked status not filled!");
                
            Response response = new()
            {
                chapters = DatabaseContexts.Quests.Chapters
                    .Select(c => Chapter.GetFromDatabase(c, player))
                    .ToDictionary(c => c.id, c => c)
            };

            return response;
        }

        public class Response
        {
            public IDictionary<long, Chapter> chapters { get; set; } = new Dictionary<long, Chapter>();
        }
    }
}