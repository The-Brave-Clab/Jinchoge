using System;
using Titanium.Web.Proxy.Http;
using Yuyuyui.PrivateServer;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.AccountTransfer;

public class SessionsEntity : BaseEntity<SessionsEntity>
{
    public SessionsEntity(Uri requestUri, string httpMethod, RouteConfig config)
        : base(requestUri, httpMethod, config)
    {
    }

    public override void ProcessRequest(byte[] requestBody,
        HeaderCollection requestHeaders,
        ref AccountTransferProxyCallbacks.PlayerSession playerSession)
    {
        var request = Deserialize<PrivateServer.SessionsEntity.Request>(requestBody)!;

        playerSession.player = new()
        {
            id = new()
            {
                uuid = request.uuid
            }
        };
    }

    public override void ProcessResponse(byte[] responseBody,
        HeaderCollection responseHeaders,
        ref AccountTransferProxyCallbacks.PlayerSession playerSession)
    {
        var response = Deserialize<PrivateServer.SessionsEntity.Response>(responseBody)!;

        if (PlayerProfile.Exists(response.code))
        {
            PlayerProfile loadedProfile = PlayerProfile.Load(response.code);
            loadedProfile.id.uuid = playerSession.player!.id.uuid;
            playerSession.player = loadedProfile;
        }
        else
        {
            PlayerProfile newProfile = 
                PrivateServer.PrivateServer.RegisterNewPlayer(playerSession.player!.id.uuid, response.code);
            newProfile.id.code = response.code;
            playerSession.player = newProfile;
        }
        playerSession.player.Save();

        Utils.LogTrace(string.Format(Resources.LOG_AT_GOT_UUID, playerSession.player.id.uuid));
        Utils.LogTrace(string.Format(Resources.LOG_AT_GOT_CODE, playerSession.player.id.code));

        playerSession.sessionID = response.session_id;
        playerSession.sessionKey = response.gk_key;
    }
}