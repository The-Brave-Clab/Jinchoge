using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class AccessoryEnhancementResultEntity : BaseEntity<AccessoryEnhancementResultEntity>
    {
        public AccessoryEnhancementResultEntity(
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

            Request requestObj = Deserialize<Request>(requestBody)!;

            Accessory playerAccessory = Accessory.Load(requestObj.id);
            
            using var accessoriesDb = new AccessoriesContext();

            DataModel.Accessory masterAccessory = 
                accessoriesDb.Accessories.First(ua => ua.Id == playerAccessory.master_id);
            DataModel.AccessoryLevel accessoryTargetLevel =
                accessoriesDb.AccessoryLevels
                    .Where(al => al.Rarity == masterAccessory.Rarity)
                    .First(al => al.Level == requestObj.accessory.level);
            
            // change player accessory status
            playerAccessory.level = requestObj.accessory.level;
            playerAccessory.quantity -= accessoryTargetLevel.NeedAmount; // don't need to clamp here
            playerAccessory.Save();
            Utils.Log($"player accessory {playerAccessory.id} level is {playerAccessory.level}");
            Utils.Log($"player accessory {playerAccessory.id} quantity -{accessoryTargetLevel.NeedAmount}");
            
            // brave coins
            if (accessoryTargetLevel.BraveCoin > 0)
            {
                player.data.braveCoin += accessoryTargetLevel.BraveCoin;
                Utils.Log($"player {player.id.code} brave coins +{accessoryTargetLevel.BraveCoin}");
            }
            // money
            player.data.money -= accessoryTargetLevel.Money;
            Utils.Log($"player {player.id.code} money -{accessoryTargetLevel.Money}");
            player.Save();

            Response responseObj = new()
            {
                accessory = AccessoryListEntity.Response.Accessory.FromPlayerAccessory(accessoriesDb, playerAccessory),
                brave_coin = accessoryTargetLevel.BraveCoin
            };
 
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Request
        {
            public AccessoryEnhancementData accessory { get; set; } = new();
            public long id { get; set; } // user accessory id

            public class AccessoryEnhancementData
            {
                public int level { get; set; }
            }
        }

        public class Response
        {
            public AccessoryListEntity.Response.Accessory accessory { get; set; } = new();
            public int brave_coin { get; set; }
        }
    }
}