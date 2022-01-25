using Newtonsoft.Json;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class EnhancementItemsEntity : BaseEntity<EnhancementItemsEntity>
    {
        public EnhancementItemsEntity(
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
            
            Utils.LogWarning("Items are stubbed for testing!");

            using var itemsDb = new ItemsContext();

            Response responseObj = new()
            {
                enhancement_items = itemsDb.EnhancementItems
                    .Where(i => i.Id < 10000)
                    .Select(i => new Response.EnhancementItem
                    {
                        id = 1000000 + i.Id,
                        master_id = i.Id,
                        quantity = 60,
                        active_skill_level_potential = i.ActiveSkillLevelPotential,
                        rarity = i.Rarity,
                        available_character_1_id = i.AvailableCharacterId1,
                        available_character_2_id = i.AvailableCharacterId2,
                        pair_limited = i.AvailableCharacterId1 != null && i.AvailableCharacterId2 != null,
                        priority = i.Priority,
                        support_skill_level_potential = i.SupportSkillLevelPotential,
                        support_skill_level_category = i.SupportSkillLevelCategory
                    })
                    .ToList()
            };

            var responseStr = JsonConvert.SerializeObject(responseObj, Formatting.Indented);
            

            // Response responseObj = new()
            // {
            //     enhancement_items = player.items.enhancement
            //         .Select(p => p.Value)
            //         .ToDictionary(c => c, Item.Load)
            // };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IList<EnhancementItem> enhancement_items { get; set; }
                = new List<EnhancementItem>();

            public class EnhancementItem
            {
                public long id { get; set; }
                public long master_id { get; set; }
                public int quantity { get; set; }
                public int active_skill_level_potential { get; set; }
                public int rarity { get; set; }
                public long? available_character_1_id { get; set; } = null;
                public long? available_character_2_id { get; set; } = null;
                public bool pair_limited { get; set; }
                public long priority { get; set; }
                public int support_skill_level_potential { get; set; }
                public int support_skill_level_category { get; set; }
            }
        }
    }
}