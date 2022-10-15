using System;
using Titanium.Web.Proxy.Http;
using Yuyuyui.PrivateServer;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.AccountTransfer;

public class HeaderEntity : BaseEntity<HeaderEntity>
{
    public HeaderEntity(Uri requestUri, string httpMethod, RouteConfig config)
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
        var response = Deserialize<PrivateServer.HeaderEntity.Response>(responseBody)!;

        playerSession.player!.data.level = response.header.level;
        playerSession.player!.data.exp = response.header.exp;
        playerSession.player!.data.money = response.header.money;
        playerSession.player!.data.friendPoint = response.header.friend_point;
        playerSession.player!.data.braveCoin = response.header.brave_coin;
        playerSession.player!.data.titleItemID = response.header.title_item_id;
        /* Taisha Point */
        playerSession.player!.data.exchangePoint = response.header.exchange_point;
        playerSession.player!.data.stamina = response.header.stamina;
        playerSession.player!.data.weekdayStamina = response.header.weekday_stamina;
        playerSession.player!.data.tutorialProgress = 1000; // the player must have completed the tutorial already

        playerSession.player.Save();

        Utils.LogTrace(string.Format(Resources.LOG_AT_GOT_HEADER,
            playerSession.player!.data.level,
            playerSession.player!.data.exp,
            playerSession.player!.data.money));
    }
}