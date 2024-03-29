using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer
{
    public class AccessoryListEntity : BaseEntity<AccessoryListEntity>
    {
        public AccessoryListEntity(
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

            if (player.accessories.Count == 0)
            {
                var newGyuuki = Accessory.DefaultAccessory();
                player.accessories.Add(newGyuuki.master_id, newGyuuki.id);
                newGyuuki.Save();
                player.Save();
                Utils.Log(Resources.LOG_PS_ACCESSORY_ASSIGN_DEFAULT);
            }

            using var accessoriesDb = new AccessoriesContext();
            
            Response responseObj = new()
            {
                accessories = player.accessories
                    .Select(a => Accessory.Load(a.Value))
                    .Select(a => Response.Accessory.FromPlayerAccessory(accessoriesDb, a))
                    .ToDictionary(p => $"{p.id}", p => p)
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IDictionary<string, Accessory> accessories { get; set; } = new Dictionary<string, Accessory>();

            public class Accessory
            {
                public long id { get; set; }
                public long master_id { get; set; }
                public int level { get; set; }
                public int cost { get; set; }
                public int hit_point { get; set; }
                public int attack { get; set; }
                public long money { get; set; }
                public int quantity { get; set; }
                public int next_quantity { get; set; }

                public static Accessory FromPlayerAccessory(AccessoriesContext accessoriesDb, 
                    Yuyuyui.PrivateServer.Accessory playerAccessory)
                {
                    DataModel.Accessory masterAccessory = 
                        accessoriesDb.Accessories.First(ua => ua.Id == playerAccessory.master_id);
                    float growthKindValue = GrowthKind.GetValue(3); // growth kind for accessories is fixed 3
                    DataModel.AccessoryLevel accessoryNextLevel =
                        accessoriesDb.AccessoryLevels
                            .Where(al => al.Rarity == masterAccessory.Rarity)
                            // it seems that the max level of accessories (seireis) is 20
                            // consider changing the hardcoded value
                            .First(al => al.Level == Math.Min(playerAccessory.level + 1, 20));
                    return new()
                    {
                        id = playerAccessory.id,
                        master_id = playerAccessory.master_id,
                        level = playerAccessory.level,
                        cost = CalcUtil.CalcParamByLevel(
                            playerAccessory.level, 1, masterAccessory.MaxLevel, 
                            masterAccessory.MinCost, masterAccessory.MaxCost, growthKindValue),
                        hit_point = CalcUtil.CalcParamByLevel(
                            playerAccessory.level, 1, masterAccessory.MaxLevel, 
                            masterAccessory.MinHitPoint, masterAccessory.MaxHitPoint, growthKindValue),
                        attack = CalcUtil.CalcParamByLevel(
                            playerAccessory.level, 1, masterAccessory.MaxLevel, 
                            masterAccessory.MinAttack, masterAccessory.MaxAttack, growthKindValue),
                        money = accessoryNextLevel.Money,
                        quantity = playerAccessory.quantity,
                        next_quantity = accessoryNextLevel.NeedAmount,
                    };
                }
            }
        }
    }
}