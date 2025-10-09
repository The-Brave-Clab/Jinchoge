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
            var player = await GetPlayerFromCookies();

            // remove the gifts that have been accepted for 14 days
            // or exceeds the limit of 20
            List<Gift> giftsToBeRemoved = [];
            var acceptedGifts = (await Gift.LoadMany(player.acceptedGifts)).ToArray();
            foreach (var gift in acceptedGifts)
            {
                var timePassed = DateTime.UtcNow - Utils.FromUnixTime(gift.received_at).ToUniversalTime();
                if (timePassed.TotalDays > 14.0)
                {
                    if (!giftsToBeRemoved.Contains(gift))
                        giftsToBeRemoved.Add(gift);
                }
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

            var gifts = await Gift.LoadMany(player.acceptedGifts);
            PresentsEntity.Response responseObj = new()
            {
                gifts = gifts.ToList()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }
    }
}