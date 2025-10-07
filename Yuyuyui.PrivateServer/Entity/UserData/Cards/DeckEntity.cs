using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer
{
    public class DeckEntity : BaseEntity<DeckEntity>
    {
        public DeckEntity(
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
            var player = await GetPlayerFromCookies();

            if (player.decks.Count == 0)
            {
                // when player's deck is empty, it has to be a new player
                // which means there has to be these four cards.
                var yuuna = await Card.Load(player.cards[100010]);
                var tougou = await Card.Load(player.cards[100020]);
                var fuu = await Card.Load(player.cards[100040]);
                var itsuki = await Card.Load(player.cards[100050]);

                var yuunaUnit = await yuuna.CreateUnit(tougou.AsSupport());
                var fuuUnit = await fuu.CreateUnit();
                var itsukiUnit = await itsuki.CreateUnit();

                await yuunaUnit.Save();
                await fuuUnit.Save();
                await itsukiUnit.Save();

                var firstDeck = new Deck
                {
                    id = await Deck.GetID(),
                    leaderUnitID = yuunaUnit.id,
                    name = null,
                    units = new List<long> {yuunaUnit.id, fuuUnit.id, itsukiUnit.id}
                };
                await firstDeck.Save();

                player.decks.Add(firstDeck.id);

                for (int i = 1; i < 14; ++i)
                {
                    var unit1 = await yuuna.CreateUnit();
                    var unit2 = await Unit.CreateEmptyUnit();
                    var unit3 = await Unit.CreateEmptyUnit();
                    await unit1.Save();
                    await unit2.Save();
                    await unit3.Save();

                    var deck = new Deck
                    {
                        id = await Deck.GetID(),
                        leaderUnitID = unit1.id,
                        name = null,
                        units = new List<long> {unit1.id, unit2.id, unit3.id}
                    };
                    await deck.Save();

                    player.decks.Add(deck.id);
                }

                await player.Save();
                Utils.Log(Resources.LOG_PS_DECK_SET_DEFAULT);
            }

            var decks = await player.decks
                .Select(Deck.Load)
                .WhenAll();
            var responseDeck = await decks
                .Select(d => Response.Deck.FromPlayerDeck(d, player))
                .WhenAll();
            Response responseObj = new()
            {
                decks = responseDeck.ToList()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }

        public class Response
        {
            public IList<Deck> decks { get; set; } = new List<Deck>();

            public class Deck
            {
                public long id { get; set; }
                public long leader_deck_card_id { get; set; }
                public string? name { get; set; } = null;
                public IList<Unit.CardWithSupport> cards { get; set; } = new List<Unit.CardWithSupport>();

                public static async Task<Deck> FromPlayerDeck(Yuyuyui.PrivateServer.Deck d, PlayerProfile player)
                {
                    var units = await d.units
                        .Select(Unit.Load)
                        .WhenAll();
                    var cardsWithSupport = await units
                        .Select(u => Unit.CardWithSupport.FromUnit(u, player))
                        .WhenAll();
                    return new Response.Deck
                    {
                        id = d.id,
                        leader_deck_card_id = d.leaderUnitID,
                        name = d.name,
                        cards = cardsWithSupport.ToList()!
                    };
                }
            }
        }
    }
}