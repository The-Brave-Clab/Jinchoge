using System.Reflection;

namespace Yuyuyui.PrivateServer
{
    public class DeckEntity : BaseEntity<DeckEntity>
    {
        public DeckEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();
            
            if (player.decks.Count == 0)
            {
                // when player's deck is empty, it has to be a new player
                // which means there has to be four cards, and they are in order.
                var yuuna = Card.Load($"{player.cards[0]}");
                var tougou = Card.Load($"{player.cards[1]}");
                var fuu = Card.Load($"{player.cards[2]}");
                var itsuki = Card.Load($"{player.cards[3]}");

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

            Response responseObj = new()
            {
                decks = player.decks
                    .Select(id => Deck.Load($"{id}"))
                    .Select(d =>
                    {
                        return new Response.Deck
                        {
                            id = d.id,
                            leader_deck_card_id = d.leaderUnitID,
                            name = d.name,
                            cards = d.units.Select(id => (Unit.CardWithSupport) Unit.Load($"{id}")!).ToList()
                        };
                    }).ToList()
            };

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
            }
        }
    }
}