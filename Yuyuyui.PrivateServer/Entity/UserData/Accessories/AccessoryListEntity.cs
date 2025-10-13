using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer;

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

    protected override async Task ProcessRequest()
    {
        var playerId = await GetPlayerIdFromCookies();

        IDictionary<long, long> playerAccessories;
        await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
        {
            var player = await PlayerProfile.Load(playerId.code);
            if (player.accessories.Count == 0)
            {
                var newGyuuki = await Accessory.DefaultAccessory();
                player.accessories.Add(newGyuuki.master_id, newGyuuki.id);
                await newGyuuki.Save();
                await player.Save();
                Utils.Log(Resources.LOG_PS_ACCESSORY_ASSIGN_DEFAULT);
            }

            playerAccessories = player.accessories;
        }

        var accessories = await playerAccessories
            .Select(a => Accessory.Load(a.Value))
            .WhenAll();

        Response responseObj = new()
        {
            accessories = accessories
                .Select(Response.Accessory.FromPlayerAccessory)
                .ToDictionary(p => $"{p.id}", p => p)
        };

        responseBody = Serialize(responseObj);
        SetBasicResponseHeaders();
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

            public static Accessory FromPlayerAccessory(Yuyuyui.PrivateServer.Accessory playerAccessory)
            {
                DataModel.Accessory masterAccessory;
                AccessoryLevel accessoryNextLevel;

                using (AccessoriesContext accessoriesDb = new())
                {
                    masterAccessory = accessoriesDb.Accessories.First(ua => ua.Id == playerAccessory.master_id);
                    accessoryNextLevel = accessoriesDb.AccessoryLevels
                        .Where(al => al.Rarity == masterAccessory.Rarity)
                        // it seems that the max level of accessories (seireis) is 20
                        // consider changing the hardcoded value
                        .First(al => al.Level == Math.Min(playerAccessory.level + 1, 20));
                }

                float growthKindValue = GrowthKind.GetValue(3); // growth kind for accessories is fixed 3
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