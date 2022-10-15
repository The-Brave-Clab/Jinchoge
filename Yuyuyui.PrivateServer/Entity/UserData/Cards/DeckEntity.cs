using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

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

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();

            if (player.decks.Count == 0)
            {
                // when player's deck is empty, it has to be a new player
                // which means there has to be these four cards.
                var yuuna = Card.Load(player.cards[100010]);
                var tougou = Card.Load(player.cards[100020]);
                var fuu = Card.Load(player.cards[100040]);
                var itsuki = Card.Load(player.cards[100050]);

                var yuunaUnit = yuuna.CreateUnit(tougou.AsSupport());
                var fuuUnit = fuu.CreateUnit();
                var itsukiUnit = itsuki.CreateUnit();

                yuunaUnit.Save();
                fuuUnit.Save();
                itsukiUnit.Save();

                var firstDeck = new Deck
                {
                    id = Deck.GetID(),
                    leaderUnitID = yuunaUnit.id,
                    name = null,
                    units = new List<long> {yuunaUnit.id, fuuUnit.id, itsukiUnit.id}
                };
                firstDeck.Save();

                player.decks.Add(firstDeck.id);

                for (int i = 1; i < 14; ++i)
                {
                    var unit1 = yuuna.CreateUnit();
                    var unit2 = Unit.CreateEmptyUnit();
                    var unit3 = Unit.CreateEmptyUnit();
                    unit1.Save();
                    unit2.Save();
                    unit3.Save();

                    var deck = new Deck
                    {
                        id = Deck.GetID(),
                        leaderUnitID = unit1.id,
                        name = null,
                        units = new List<long> {unit1.id, unit2.id, unit3.id}
                    };
                    deck.Save();

                    player.decks.Add(deck.id);
                }

                player.Save();
                Utils.Log("Assigned default decks to player.");
            }

            Utils.LogWarning("Unit calculation is not finished yet!");

            Response responseObj;
            using (var cardsDb = new CardsContext())
            using (var charactersDb = new CharactersContext())
            {
                responseObj = new()
                {
                    decks = player.decks
                        .Select(Deck.Load)
                        .Select(d => Response.Deck.FromPlayerDeck(cardsDb, charactersDb, d, player))
                        .ToList()
                };
            }

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
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

                public static Deck FromPlayerDeck(CardsContext cardsDb, CharactersContext charactersDb,
                    Yuyuyui.PrivateServer.Deck d, PlayerProfile player)
                {
                    return new Response.Deck
                    {
                        id = d.id,
                        leader_deck_card_id = d.leaderUnitID,
                        name = d.name,
                        cards = d.units.Select(id =>
                                Unit.CardWithSupport.FromUnit(cardsDb, charactersDb, Unit.Load(id), player))
                            .ToList()!
                    };
                }
            }
        }
    }
}