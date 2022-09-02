using System.Text;
using Yuyuyui.PrivateServer.DataModel;
using Yuyuyui.PrivateServer.Strategy;

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
                PostRequest requestObj = Deserialize<PostRequest>(requestBody)!;

                player.data.titleItemID = requestObj.title_item_id;
                player.Save();

                // responseBody = Serialize(postResponseObj);
                
                // It seems that the client doesn't read the response
                responseBody = Encoding.UTF8.GetBytes("{}");
            }
            else
            {
                if (!player.items.titleItems.Any())
                {
                    InitDefaultTitleItem(player);
                }
                
                AddEligibleCardTitleToPlayerProfile(player);

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

        private static void AddEligibleCardTitleToPlayerProfile(PlayerProfile player)
        {
            IQueryable<TitleItem> eligibleCardTitleItems = new ObtainableCardTitleDeterminationStrategy().Determine(player);
            eligibleCardTitleItems.ForEach(titleItem => player.items.titleItems.Add(titleItem.Id));
            player.Save();
        }

        private static void InitDefaultTitleItem(PlayerProfile player)
        {
            player.items.titleItems.Add(90001);
            player.Save();
        }

        public class PostRequest
        {
            public long title_item_id { get; set; } // master id of the title
        }

        // public class PostResponse
        // {
        //     public UpdateUserTitle update_user_title { get; set; }
        //
        //     public class UpdateUserTitle
        //     {
        //         public long id { get; set; } // an internal user id
        //         public long title_item_id { get; set; } // master id of the title
        //         public int level { get; set; } // user level
        //         public string nickname { get; set; } // user nickname
        //         public string comment { get; set; } // user comment
        //         public string uuid { get; set; } // user uuid
        //         public string app_version { get; set; }
        //         public string device { get; set; } // cookies X-APP-DEVICE
        //         public string kind { get; set; } // Only saw "noraml", what is this?
        //         public long leader_deck_card_id { get; set; }
        //         public int user_fellowships_count { get; set; }
        //         public int platform_number { get; set; } // An enum? 2 on iOS
        //         public string accessed_at { get; set; } // UTC time
        //         public string stamina_full_recover_at { get; set; }
        //         public string created_at { get; set; } // account creation time
        //         public string updated_at { get; set; }
        //     }
        // }

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