using Titanium.Web.Proxy.Http;
using Yuyuyui.PrivateServer;
using Yuyuyui.PrivateServer.DataModel;
using Card = Yuyuyui.PrivateServer.Card;

namespace Yuyuyui.AccountTransfer
{
    public class CardsEntity : BaseEntity<CardsEntity>
    {
        public CardsEntity(
            Uri requestUri,
            string httpMethod,
            Config config)
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
            var response = Deserialize<PrivateServer.CardsEntity.Response>(responseBody)!;

            using (var cardsDb = new CardsContext())
            {
                foreach (var c in response.cards)
                {
                    var spIncrement = cardsDb.Cards
                        .Where(card => card.Id == c.Value.master_id)
                        .Select(card => c.Value.support_point - card.SupportPoint)
                        .ToList()
                        .FirstOrDefault(0);
                    
                    Card card = new()
                    {
                        id = c.Value.id,
                        master_id = c.Value.master_id,
                        level = c.Value.level,
                        exp = c.Value.exp,
                        potential = c.Value.potential,
                        active_skill_level = c.Value.active_skill_level,
                        support_skill_level = c.Value.support_skill_level,
                        evolution_level = c.Value.evolution_level,
                        base_sp_increment = spIncrement
                    };
                    card.Save();

                    playerSession.player!.cards[card.master_id] = card.id;
                }
            }

            playerSession.player!.Save();
            
            Utils.LogTrace($"Got cards, {response.cards.Count} in total.");
        }
    }
}