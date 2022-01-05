using System.Text;

namespace Yuyuyui.PrivateServer
{
    public class EventBonusCardsEntity : BaseEntity<EventBonusCardsEntity>
    {
        public EventBonusCardsEntity(
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
            Utils.LogWarning("Stub API!");
            
            Response responseObj = new()
            {
                event_bonus_cards = new List<Response.EventBonusCard>()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }
        public class Response
        {
            public IList<EventBonusCard> event_bonus_cards { get; set; } = new List<EventBonusCard>();

            public class EventBonusCard
            {
                public long special_chapter_id { get; set; } // event_stories/special_chapters/id
                public long master_id { get; set; } // card id
                public long content_id { get; set; } // event_stories/special_episodes/id
                public float effect_rate { get; set; } // only seeing 0.5 for now?
            }
        }
    }
}