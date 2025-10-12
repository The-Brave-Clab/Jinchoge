using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class PresentsHistoryEntity : BaseEntity<PresentsHistoryEntity>
    {
        public PresentsHistoryEntity(
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

            IList<long> playerAcceptedGifts;
            await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
            {
                var player = await PlayerProfile.Load(playerId.code);

                // remove the gifts that have been accepted for 14 days
                // or exceeds the limit of 20
                List<Gift> giftsToBeRemoved = [];
                var acceptedGifts = (await Gift.LoadMany(player.acceptedGifts)).ToArray();
                foreach (var gift in acceptedGifts)
                {
                    var timePassed = DateTime.UtcNow - Utils.FromUnixTime(gift.received_at).ToUniversalTime();
                    if (!(timePassed.TotalDays > 14.0)) continue;

                    if (!giftsToBeRemoved.Contains(gift))
                        giftsToBeRemoved.Add(gift);
                }

                for (int i = 0; i < acceptedGifts.Length - 20; ++i)
                {
                    if (!giftsToBeRemoved.Contains(acceptedGifts[i]))
                        giftsToBeRemoved.Add(acceptedGifts[i]);
                }

                giftsToBeRemoved.ForEach(g => player.receivedGifts.Remove(g.id));
                await giftsToBeRemoved.ForEachAsync(g => g.Delete());

                if (giftsToBeRemoved.Count > 0)
                    await player.Save();

                playerAcceptedGifts = player.acceptedGifts;
            }

            var gifts = await Gift.LoadMany(playerAcceptedGifts);
            PresentsEntity.Response responseObj = new()
            {
                gifts = gifts.ToList()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }
    }
}