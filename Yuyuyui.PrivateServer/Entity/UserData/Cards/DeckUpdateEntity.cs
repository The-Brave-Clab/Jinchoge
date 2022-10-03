namespace Yuyuyui.PrivateServer
{
    public class DeckUpdateEntity : BaseEntity<DeckUpdateEntity>
    {
        public DeckUpdateEntity(
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

            Request requestObj = Deserialize<Request>(requestBody)!;

            Deck targetDeck = Deck.Load(requestObj.deck.id);
            targetDeck.name = requestObj.deck.name;

            foreach (var unitUpdateRequest in requestObj.deck.cards)
            {
                Unit targetUnit = Unit.Load(unitUpdateRequest.deck_card_id);
                targetUnit.baseCardID = unitUpdateRequest.user_card_id;
                targetUnit.supportCardID = unitUpdateRequest.support_user_card_id;
                targetUnit.supportCard2ID = unitUpdateRequest.support_user_card_2_id;
                targetUnit.assistCardID = unitUpdateRequest.assist_user_card_id;
                targetUnit.accessories = unitUpdateRequest.accessory_ids ??= new List<long>();
                targetUnit.Save();
            }

            targetDeck.Save();

            Response responseObj = new()
            {
                deck = DeckEntity.Response.Deck.FromPlayerDeck(targetDeck, player)
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Request
        {
            public Deck deck { get; set; } = new();

            public class Deck
            {
                public IList<UpdateDeckCard> cards { get; set; } = new List<UpdateDeckCard>();
                public long id { get; set; }
                public string? name { get; set; } = "";

                public class UpdateDeckCard
                {
                    public long setId { get; set; } // ignored
                    public long deckId { get; set; } // ignored
                    public long deck_card_id { get; set; } // unit id
                    public long? user_card_id { get; set; } = null; // leader card id
                    public long? support_user_card_id { get; set; } = null;
                    public long? support_user_card_2_id { get; set; } = null;
                    public long? assist_user_card_id { get; set; } = null;
                    public IList<long>? accessory_ids { get; set; } = null;
                }
            }
        }

        public class Response
        {
            public DeckEntity.Response.Deck deck { get; set; } = new();
        }
    }
}