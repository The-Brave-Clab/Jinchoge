using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class StaminaItemTransactionUpdateEntity : BaseEntity<StaminaItemTransactionUpdateEntity>
    {
        public StaminaItemTransactionUpdateEntity(
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
            long staminaItemId = long.Parse(GetPathParameter("stamina_item_id"));
            long transactionId = long.Parse(GetPathParameter("transaction_id"));

            if (!player.transactions.staminaItemTransactions.TryGetValue(transactionId, out var storedTransaction))
                throw new APIErrorException("A1321", $"Unknown stamina item transaction {transactionId}.");

            if (storedTransaction.stamina_item_id != staminaItemId)
                throw new APIErrorException("A1321", $"Stamina item transaction {transactionId} does not match item {staminaItemId}.");

            Item? userItem = ResolveUserItem(player, staminaItemId);
            long masterItemId = userItem?.master_id ?? staminaItemId;

            if (!storedTransaction.completed)
            {
                using var itemsDb = new ItemsContext();
                StaminaItem? masterItem = itemsDb.StaminaItems.FirstOrDefault(i => i.Id == masterItemId);
                if (masterItem == null)
                    throw new APIErrorException("A1321", $"Unknown stamina item master {masterItemId}.");

                if (!Config.Get().InGame.InfiniteItems)
                {
                    if (userItem == null || userItem.quantity <= 0)
                        throw new APIErrorException("A1321", $"Not enough stamina item {staminaItemId}.");

                    userItem.quantity -= 1;
                    userItem.Save();
                }

                player.data.stamina += masterItem.Stamina;
                storedTransaction.completed = true;
                player.Save();
            }

            responseBody = Serialize(new Response
            {
                transaction = new Transaction { id = transactionId },
                stamina = player.data.stamina,
                completed = true
            });
            SetBasicResponseHeaders();
            return Task.CompletedTask;
        }

        private static Item? ResolveUserItem(PlayerProfile player, long staminaItemId)
        {
            if (player.items.stamina.TryGetValue(staminaItemId, out long userItemId) && Item.Exists(userItemId))
                return Item.Load(userItemId);

            if (!player.items.stamina.Values.Contains(staminaItemId) || !Item.Exists(staminaItemId))
                return null;

            Item item = Item.Load(staminaItemId);
            return player.items.stamina.TryGetValue(item.master_id, out long mappedItemId) && mappedItemId == staminaItemId
                ? item
                : null;
        }

        public class Response
        {
            public Transaction transaction { get; set; } = new();
            public int stamina { get; set; }
            public bool completed { get; set; }
        }

        public class Transaction
        {
            public long id { get; set; }
        }
    }
}
