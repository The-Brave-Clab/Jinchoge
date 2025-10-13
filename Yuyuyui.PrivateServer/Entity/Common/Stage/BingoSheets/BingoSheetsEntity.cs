using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer;

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
        // var playerId = await GetPlayerIdFromCookies();
        //
        // PlayerProfile player;
        // await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
        // {
        //     player = await PlayerProfile.Load(playerId.code);
        // }

        Response responseObj = GetBingoSheets();

        responseBody = Serialize(responseObj);
        SetBasicResponseHeaders();

        return Task.CompletedTask;
    }

    private Response GetBingoSheets()
    {
        // var player = GetPlayerFromCookies();

        List<long> cartoonIds;
        using (CartoonsContext cartoonsDb = new())
        {
            cartoonIds = cartoonsDb.BingoSheets.Select(s => s.Id).ToList();
        }

        Response response = new()
        {
            bingo_sheets = cartoonIds
                .ToDictionary(id => $"{id}", id => new Response.BingoSheet
                {
                    bingo_sheet_id = id,
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