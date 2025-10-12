using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class PresentsEntity : BaseEntity<PresentsEntity>
    {
        public PresentsEntity(
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
            var playerId = await GetPlayerIdFromCookies();

            IList<long> playerReceivedGifts;
            await using (await IDistributedLockProvider.ActiveProvider!.AcquirePlayerProfileLock(playerId.code))
            {
                PlayerProfile player = await PlayerProfile.Load(playerId.code);

                // remove the gifts that have exceeded the time limit
                List<Gift> giftsToBeRemoved = [];
                var acceptedGifts = await Gift.LoadMany(player.receivedGifts);
                foreach (var gift in acceptedGifts)
                {
                    if (Utils.CurrentUnixTime() > gift.receivable_at)
                    {
                        if (!giftsToBeRemoved.Contains(gift))
                            giftsToBeRemoved.Add(gift);
                    }
                }

                giftsToBeRemoved.ForEach(g => player.receivedGifts.Remove(g.id));
                await giftsToBeRemoved.ForEachAsync(g => g.Delete());

                if (giftsToBeRemoved.Count > 0)
                    await player.Save();

                playerReceivedGifts = player.receivedGifts;
            }

            var gifts = await Gift.LoadMany(playerReceivedGifts);
            Response responseObj = new()
            {
                gifts = gifts.ToList()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }

        public class Response
        {
            public IList<Gift> gifts { get; set; } = new List<Gift>();
        }
    }
}