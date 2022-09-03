using Titanium.Web.Proxy.Http;
using Yuyuyui.PrivateServer;

namespace Yuyuyui.AccountTransfer;

public class SessionsEntity : BaseEntity<SessionsEntity>
{
    public SessionsEntity(Uri requestUri, string httpMethod, Config config)
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
        
        Utils.LogTrace($"Got user UUID: {request.uuid}");
        
        TransferProgress.Completed(TransferProgress.TaskType.UUID);
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

        Utils.LogTrace($"Got user code: {response.code}");

        playerSession.sessionID = response.session_id;
        playerSession.sessionKey = response.gk_key;

        TransferProgress.Completed(TransferProgress.TaskType.Code);
    }
}