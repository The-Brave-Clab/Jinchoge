namespace Yuyuyui.PrivateServer
{
    public class PresentsEntity : BaseEntity<PresentsEntity>
    {
        public PresentsEntity(
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
            
            // remove the gifts that have exceeded the time limit
            List<Gift> giftsToBeRemoved = new List<Gift>();
            var acceptedGifts = player.receivedGifts.Select(Gift.Load).ToList();
            foreach (var gift in acceptedGifts)
            {
                if (Utils.CurrentUnixTime() > gift.receivable_at)
                {
                    if (!giftsToBeRemoved.Contains(gift))
                        giftsToBeRemoved.Add(gift);
                }
            }

            foreach (var gift in giftsToBeRemoved)
            {
                gift.Delete();
                player.receivedGifts.Remove(gift.id);
            }
            if (giftsToBeRemoved.Count > 0)
                player.Save();

            Response responseObj = new()
            {
                gifts = player.receivedGifts.Select(Gift.Load).ToList()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IList<Gift> gifts { get; set; } = new List<Gift>();
        }
    }
}