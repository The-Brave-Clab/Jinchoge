using System;
using System.Linq;
using Microsoft.Extensions.DependencyModel;
using Titanium.Web.Proxy.Http;
using Yuyuyui.PrivateServer;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.AccountTransfer
{
    public class DeckEntity : BaseEntity<DeckEntity>
    {
        public DeckEntity(
            Uri requestUri,
            string httpMethod,
            RouteConfig config)
            : base(requestUri, httpMethod, config)
        {
        }

        public override void ProcessRequest(byte[] requestBody, HeaderCollection requestHeaders,
            ref AccountTransferProxyCallbacks.PlayerSession playerSession)
        {
        }

        public override void ProcessResponse(byte[] responseBody, HeaderCollection responseHeaders,
            ref AccountTransferProxyCallbacks.PlayerSession playerSession)
        {
            var response = Deserialize<PrivateServer.DeckEntity.Response>(responseBody)!;

            playerSession.player!.decks = response.decks.Select(d =>
            {
                Deck deck = new Deck
                {
                    id = d.id,
                    leaderUnitID = d.leader_deck_card_id,
                    name = d.name,
                    units = d.cards.Select(u =>
                    {
                        Unit unit = new Unit
                        {
                            id = u.id,
                            baseCardID = u.user_card_id,
                            supportCardID = u.support.ContainsKey("user_card_id") ? u.support["user_card_id"] : null,
                            supportCard2ID = u.support_2.ContainsKey("user_card_id") ? u.support_2["user_card_id"] : null,
                            assistCardID = u.assist.ContainsKey("user_card_id") ? u.assist["user_card_id"] : null,
                            accessories = u.accessories.Select(a => a.id).ToList()
                        };
                        unit.Save();
                        return u.id;
                    }).ToList()
                };

                deck.Save();

                return d.id;
            }).ToList();

            playerSession.player!.Save();
            
            Utils.LogTrace(Resources.LOG_AT_GOT_DECKS);
        }
    }
}