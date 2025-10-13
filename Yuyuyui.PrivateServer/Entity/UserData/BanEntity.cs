using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer;

public class BanEntity : BaseEntity<BanEntity>
{
    public BanEntity(
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
        var playerId = await GetPlayerIdFromCookies();

        await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
        {
            var player = await PlayerProfile.Load(playerId.code);
            await player.BanAccount();
        }

        responseBody = "{}"u8.ToArray();
        SetBasicResponseHeaders();
    }
}