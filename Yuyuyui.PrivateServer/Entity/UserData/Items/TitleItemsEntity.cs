using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class TitleItemsEntity : BaseEntity<TitleItemsEntity>
    {
        public TitleItemsEntity(
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

            using var itemsDb = new ItemsContext();

            if (HttpMethod == "POST")
            {
                
            }
            else
            {
                GetResponse responseObj = new()
                {
                    title_items = itemsDb.TitleItems.ToList()
                        // either player has it, or it's a character title
                        .Where(t => player.items.titleItems.Contains(t.Id) || t.ContentType == 2)
                        // cache the player ownership status
                        .Select(t => new Tuple<TitleItem, bool>(t, player.items.titleItems.Contains(t.Id)))
                        .ToDictionary(t => t.Item1.Id,
                            t => new GetResponse.TitleItem
                                {
                                    id = t.Item2 ? t.Item1.Id : null, // this should be the player owned item id but we just use a master id instead
                                    master_id = t.Item1.Id,
                                    verified = t.Item2 ? 1 : null, // this should be if the player has checked the title but we just ignore it here
                                    is_grayout = !t.Item2,
                                    is_max_evo = false // this is a client not implemented feature
                                })
                };

                responseBody = Serialize(responseObj);
            }

            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class GetResponse
        {
            public IDictionary<long, TitleItem> title_items { get; set; } = 
                new Dictionary<long, TitleItem>(); // master_id as key

            public class TitleItem
            {
                public long? id { get; set; }
                public long master_id { get; set; } // from master_data
                public int? verified { get; set; } // 1 for true, null for false
                public bool is_grayout { get; set; }
                public bool is_max_evo { get; set; }
            }
        }
    }
}