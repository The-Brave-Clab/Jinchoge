using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer;

public class StaminaItemsEntity : BaseEntity<StaminaItemsEntity>
{
    public StaminaItemsEntity(
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

        bool infiniteItems;
        IDictionary<long, long> playerStaminaItems;
        await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
        {
            var player = await PlayerProfile.Load(playerId.code);
            infiniteItems = await PrivateServer.ResourceProvider.inGameConfigProvider.GetInfiniteItems(player);
            playerStaminaItems = player.items.stamina;
        }

        Response responseObj;
        if (infiniteItems)
        {
            List<StaminaItem> staminaItems;
            await using (ItemsContext itemsDb = new())
            {
                staminaItems = itemsDb.StaminaItems.ToList();
            }

            responseObj = new()
            {
                stamina_items = staminaItems
                    .Select(t => new Item
                    {
                        id = t.Id,
                        master_id = t.Id,
                        quantity = 999
                    })
                    .ToDictionary(i => i.id, i => i)
            };
        }
        else
        {
            var staminaItems = await Item.LoadMany(
                playerStaminaItems
                    .Select(p => p.Value));
            responseObj = new()
            {
                stamina_items = staminaItems
                    .Where(si => si.quantity > 0)
                    .ToDictionary(si => si.id, si => si)
            };
        }

        responseBody = Serialize(responseObj);
        SetBasicResponseHeaders();
    }

    public class Response
    {
        public IDictionary<long, Item> stamina_items { get; set; } = new Dictionary<long, Item>();
    }
}