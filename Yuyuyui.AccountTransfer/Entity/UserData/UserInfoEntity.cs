using System;
using Titanium.Web.Proxy.Http;
using Yuyuyui.PrivateServer;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.AccountTransfer;

public class UserInfoEntity : BaseEntity<UserInfoEntity>
{
    public UserInfoEntity(Uri requestUri, string httpMethod, RouteConfig config)
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
        var response = Deserialize<PrivateServer.UserInfoEntity.Response>(responseBody)!;

        if (response.user.id != playerSession.player!.id.code) return;

        playerSession.player!.profile.nickname = response.user.nickname;
        playerSession.player!.profile.comment = response.user.comment;

        playerSession.player.Save();

        Utils.LogTrace(string.Format(Resources.LOG_AT_GOT_USER_INFO,
            playerSession.player!.profile.nickname,
            playerSession.player!.profile.comment));
    }
}