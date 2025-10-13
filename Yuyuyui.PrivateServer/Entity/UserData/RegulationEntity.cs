using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer;

public class RegulationEntity : BaseEntity<RegulationEntity>
{
    public RegulationEntity(
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

        int regulationVersion;
        await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
        {
            var player = await PlayerProfile.Load(playerId.code);

            if (requestBody.Length > 0)
            {
                Request request = Deserialize<Request>(requestBody)!;
                int checkVersion = request.regulation_version.current_version;
                Utils.Log(string.Format(Resources.PS_LOG_REGULATION_AGREED, checkVersion));
                player.data.regulationVersion = checkVersion;
                await player.Save();
            }

            regulationVersion = player.data.regulationVersion;
        }

        Response responseObj = new()
        {
            regulation_version = new()
            {
                current_version = 1,
                checked_version = regulationVersion,
                regulation_url = $"{RequestAuthority}/"
            }
        };

        responseBody = Serialize(responseObj);
        SetBasicResponseHeaders();
    }

    public class Request
    {
        public RegulationVersion regulation_version { get; set; } = new();

        public class RegulationVersion
        {
            public int current_version { get; set; }
        }
    }

    public class Response
    {
        public RegulationVersion regulation_version { get; set; } = new();

        public class RegulationVersion
        {
            public int current_version { get; set; }
            public int checked_version { get; set; }
            public string regulation_url { get; set; } = "";
        }
    }
}