namespace Yuyuyui.PrivateServer
{
    public class CardsEntity : BaseEntity<CardsEntity>
    {
        public CardsEntity(
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
            
            if (player.cards.Count == 0)
            {
                var yuuna = Card.DefaultYuuna();
                var tougou = Card.DefaultTougou();
                var fuu = Card.DefaultFuu();
                var itsuki = Card.DefaultItsuki();
                player.cards.Add(yuuna.id);
                player.cards.Add(tougou.id);
                player.cards.Add(fuu.id);
                player.cards.Add(itsuki.id);
                yuuna.Save();
                tougou.Save();
                fuu.Save();
                itsuki.Save();
                player.Save();
                Utils.Log("Assigned default cards to player.");
            }

            Utils.LogWarning("Taisha point bonus not applied!");

            Response responseObj = new()
            {
                cards = player.cards.ToDictionary(c => c, Card.Load)
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IDictionary<long, Card> cards { get; set; } = new Dictionary<long, Card>();
        }
    }
}