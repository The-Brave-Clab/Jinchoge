using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            RouteConfig config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();

            using var itemsDb = new ItemsContext();

            Response responseObj = new()
            {
                enhancement_items = player.items.enhancement
                    .Select(p =>
                    {
                        EnhancementItem masterData = 
                            itemsDb.EnhancementItems.First(m => m.Id == p.Key);
                        Item userItem = Item.Load(p.Value);
                        return new Response.EnhancementItem
                        {
                            id = p.Value,
                            master_id = masterData.Id,
                            quantity = userItem.quantity,
                            active_skill_level_potential = masterData.ActiveSkillLevelPotential,
                            rarity = masterData.Rarity,
                            available_character_1_id = masterData.AvailableCharacterId1,
                            available_character_2_id = masterData.AvailableCharacterId2,
                            pair_limited = masterData.AvailableCharacterId1 != null &&
                                           masterData.AvailableCharacterId2 != null,
                            priority = masterData.Priority,
                            support_skill_level_potential = masterData.SupportSkillLevelPotential,
                            support_skill_level_category = masterData.SupportSkillLevelCategory
                        };
                    })
                    .Where(ei => ei.quantity > 0) // don't show consumed items
                    .ToList()
            };

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