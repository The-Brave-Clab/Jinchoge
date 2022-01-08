namespace Yuyuyui.PrivateServer
{
    public class PresentsHistoryEntity : BaseEntity<PresentsHistoryEntity>
    {
        public PresentsHistoryEntity(
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

            // remove the gifts that have been accepted for 14 days
            // or exceeds the limit of 20
            List<Gift> giftsToBeRemoved = new List<Gift>();
            var acceptedGifts = player.acceptedGifts.Select(Gift.Load).ToList();
            foreach (var gift in acceptedGifts)
            {
                var timePassed = DateTime.UtcNow - Utils.FromUnixTime(gift.received_at).ToUniversalTime();
                if (timePassed.TotalDays > 14.0)
                {
                    if (!giftsToBeRemoved.Contains(gift))
                        giftsToBeRemoved.Add(gift);
                }
            }

            for (int i = 0; i < acceptedGifts.Count - 20; ++i)
            {
                if (!giftsToBeRemoved.Contains(acceptedGifts[i]))
                    giftsToBeRemoved.Add(acceptedGifts[i]);
            }

            foreach (var gift in giftsToBeRemoved)
            {
                gift.Delete();
                player.acceptedGifts.Remove(gift.id);
            }
            if (giftsToBeRemoved.Count > 0)
                player.Save();

            PresentsEntity.Response responseObj = new()
            {
                gifts = player.acceptedGifts.Select(Gift.Load).ToList()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }
    }
}