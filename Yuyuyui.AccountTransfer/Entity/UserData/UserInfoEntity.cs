using Titanium.Web.Proxy.Http;
using Yuyuyui.PrivateServer;

namespace Yuyuyui.AccountTransfer;

public class UserInfoEntity : BaseEntity<UserInfoEntity>
{
    public UserInfoEntity(Uri requestUri, string httpMethod, Config config)
        : base(requestUri, httpMethod, config)
    {
    }

    public override void ProcessRequest(byte[] requestBody,
        HeaderCollection requestHeaders,
        ref AccountTransferProxyCallbacks.PlayerSession playerSession)
    {

    }

    public override void ProcessResponse(byte[] responseBody,
        HeaderCollection responseHeaders,
        ref AccountTransferProxyCallbacks.PlayerSession playerSession)
    {
        PrivateServer.UserInfoEntity.Response response = 
            Deserialize<PrivateServer.UserInfoEntity.Response>(responseBody)!;

        if (response.user.id != playerSession.player!.id.code) return;

        playerSession.player!.profile.nickname = response.user.nickname;
        playerSession.player!.profile.comment = response.user.comment;

        playerSession.player.Save();

        Utils.LogTrace("Got user profile:");
        Utils.LogTrace($"\tNickname: {playerSession.player!.profile.nickname}");
        Utils.LogTrace($"\tComment: {playerSession.player!.profile.comment}");

        TransferProgress.Completed(TransferProgress.TaskType.Profile);
    }
}