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
        Request session = Deserialize<Request>(requestBody)!;

        playerSession.player = new()
        {
            id = new()
            {
                uuid = session.uuid
            }
        };
        
        Utils.LogTrace($"Got user UUID: {session.uuid}");
        
        TransferProgress.Completed(TransferProgress.TaskType.UUID);
    }

    public override void ProcessResponse(byte[] responseBody,
        HeaderCollection responseHeaders,
        ref AccountTransferProxyCallbacks.PlayerSession playerSession)
    {
        Response session = Deserialize<Response>(responseBody)!;

        if (PlayerProfile.Exists(session.code))
        {
            PlayerProfile loadedProfile = PlayerProfile.Load(session.code);
            loadedProfile.id.uuid = playerSession.player!.id.uuid;
            playerSession.player = loadedProfile;
        }
        else
        {
            PlayerProfile newProfile = 
                PrivateServer.PrivateServer.RegisterNewPlayer(playerSession.player!.id.uuid, session.code);
            newProfile.id.code = session.code;
            playerSession.player = newProfile;
        }
        playerSession.player.Save();

        Utils.LogTrace($"Got user code: {session.code}");

        playerSession.sessionID = session.session_id;
        playerSession.sessionKey = session.gk_key;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\nWelcome, {session.code}!\n");
        Console.ResetColor();

        TransferProgress.Completed(TransferProgress.TaskType.Code);
    }

    private class Request
    {
        public string uuid { get; set; }
    }

    private class Response
    {
        public string code { get; set; }
        public string session_id { get; set; }
        public long unixtime { get; set; }
        public string gk_key { get; set; }
    }
}