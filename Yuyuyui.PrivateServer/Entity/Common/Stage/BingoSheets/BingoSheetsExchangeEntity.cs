using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class BingoSheetsExchangeEntity : BaseEntity<BingoSheetsExchangeEntity>
    {
        public BingoSheetsExchangeEntity(
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
            long bingoSheetId = long.Parse(GetPathParameter("bingo_sheet_id")); // ignored

            Request request = Deserialize<Request>(requestBody)!;
            Response responseObj;
            using (var cartoonsDb = new CartoonsContext())
                responseObj = new()
                {
                    reward_items = new List<int>(),
                    completion_reward_items = new List<int>(),
                    sheet = new()
                    {
                        open_bingo_squares = new List<Response.BingoSquare>(),
                        opened_bingo_squares = cartoonsDb.BingoSquares
                            .Where(s => s.BingoSheetId == request.bingoSheetId) // unnecessary
                            .Select(s => new Response.BingoSquare { id = s.Id })
                            .ToList(),
                        required_item_quantity = 16, // fixed?
                        current_item_quantity = null, // unknown
                        lap_count = 0 // unknown
                    }
                };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Request
        {
            public long bingoSheetId { get; set; }
        }
        public class Response
        {
            public IList<int> reward_items { get; set; } = new List<int>(); // unknown type
            public IList<int> completion_reward_items { get; set; } = new List<int>(); // unknown type
            public Sheet sheet { get; set; } = new();

            public class Sheet
            {
                public IList<BingoSquare> open_bingo_squares { get; set; } = new List<BingoSquare>(); // unknown, assumed type
                public IList<BingoSquare> opened_bingo_squares { get; set; } = new List<BingoSquare>();
                public int required_item_quantity { get; set; }
                public object? current_item_quantity { get; set; } = null; // unknown
                public int lap_count { get; set; }
            }

            public class BingoSquare
            {
                public long id { get; set; }
            }
        }
    }
}