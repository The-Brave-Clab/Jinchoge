using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class BingoSheetsEntity : BaseEntity<BingoSheetsEntity>
    {
        public BingoSheetsEntity(
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

            using var questsDb = new QuestsContext();
            Response responseObj = GetChapters(questsDb);

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        protected virtual Response GetChapters(QuestsContext questsDb)
        {
            var player = GetPlayerFromCookies();
            
            // Utils.LogWarning("Locked status not filled!");

            using var cartoonsDb = new CartoonsContext();
                
            Response response = new()
            {
                bingo_sheets = cartoonsDb.BingoSheets
                    .ToDictionary(s => $"{s.Id}", s => new Response.BingoSheet
                    {
                        bingo_sheet_id = s.Id,
                        is_openable = true,
                        is_card_gettable = false
                    })
            };

            return response;
        }

        public class Response
        {
            public IDictionary<string, BingoSheet> bingo_sheets { get; set; } = new Dictionary<string, BingoSheet>();

            public class BingoSheet
            {
                public long bingo_sheet_id { get; set; }
                public bool is_openable { get; set; }
                public bool is_card_gettable { get; set; }
            }
        }
    }
}