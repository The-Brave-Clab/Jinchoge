using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class AccessoryListEntity : BaseEntity<AccessoryListEntity>
    {
        public AccessoryListEntity(
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

            if (player.accessories.Count == 0)
            {
                var newGyuuki = Accessory.DefaultAccessory();
                player.accessories.Add(newGyuuki.master_id, newGyuuki.id);
                newGyuuki.Save();
                player.Save();
                Utils.Log("Assigned default accessory to player.");
            }

            Utils.LogWarning("Stub API!");

            using var accessoryDb = new AccessoriesContext();
            
            Response responseObj = new()
            {
                accessories = player.accessories
                    .Select(a => Accessory.Load(a.Value))
                    .Select(a =>
                    {
                        DataModel.Accessory masterAccessory = 
                            accessoryDb.Accessories.First(ua => ua.Id == a.master_id);
                        float growthKindValue = GrowthKind.GetValue(3);
                        DataModel.AccessoryLevel accessoryNextLevel =
                            accessoryDb.AccessoryLevels
                                .Where(al => al.Rarity == masterAccessory.Rarity)
                                // it seems that the max level of accessories (seireis) is 20
                                // consider changing the hardcoded value
                                .First(al => al.Level == Math.Min(a.level + 1, 20));
                        return new Response.Accessory
                        {
                            id = a.id,
                            master_id = a.master_id,
                            level = a.level,
                            cost = CalcUtil.CalcParamByLevel(
                                a.level, 1, masterAccessory.MaxLevel, 
                                masterAccessory.MinCost, masterAccessory.MaxCost, growthKindValue),
                            hit_point = CalcUtil.CalcParamByLevel(
                                a.level, 1, masterAccessory.MaxLevel, 
                                masterAccessory.MinHitPoint, masterAccessory.MaxHitPoint, growthKindValue),
                            attack = CalcUtil.CalcParamByLevel(
                                a.level, 1, masterAccessory.MaxLevel, 
                                masterAccessory.MinAttack, masterAccessory.MaxAttack, growthKindValue),
                            money = accessoryNextLevel.Money,
                            quantity = a.quantity,
                            next_quantity = accessoryNextLevel.NeedAmount,
                        };
                    })
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
                public int master_id { get; set; }
                public int level { get; set; }
                public int cost { get; set; }
                public int hit_point { get; set; }
                public int attack { get; set; }
                public long money { get; set; }
                public int quantity { get; set; }
                public int next_quantity { get; set; }
            }
        }
    }
}