using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class BillingPointTransactionUpdateEntity : BaseEntity<BillingPointTransactionUpdateEntity>
    {
        public BillingPointTransactionUpdateEntity(Uri requestUri, string httpMethod, Dictionary<string, string> requestHeaders, byte[] requestBody, RouteConfig config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config) { }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();
            long transactionId = long.Parse(GetPathParameter("transaction_id"));

            if (!player.transactions.billingPointTransactions.TryGetValue(transactionId, out var storedTransaction))
                throw new APIErrorException("A1321", $"Unknown billing point transaction {transactionId}.");

            string requestKind = GetTransactionKind();
            if (storedTransaction.kind != requestKind)
                throw new APIErrorException("A1321", $"Billing point transaction {transactionId} does not match {requestKind}.");

            if (!storedTransaction.completed)
            {
                if (requestKind == "weekday_stamina_recovery")
                {
                    if (!DeductBillingPoint(player, 50))
                        throw new APIErrorException("A1321", "Not enough billing points for weekday stamina recovery.");

                    player.data.weekdayStamina = Math.Max(player.data.weekdayStamina, 6);
                }
                else if (requestKind == "stamina_recovery")
                {
                    if (!DeductBillingPoint(player, 50))
                        throw new APIErrorException("A1321", "Not enough billing points for stamina recovery.");

                    player.data.stamina = Math.Max(player.data.stamina, 140);
                }
                else if (requestKind == "enhancement_item_capacity")
                {
                    if (!DeductBillingPoint(player, 30))
                        throw new APIErrorException("A1321", "Not enough billing points for enhancement item capacity.");

                    player.data.enhancementItemCapacity = Math.Min(player.data.enhancementItemCapacity + 10, 730);
                }

                storedTransaction.completed = true;
                player.Save();
            }

            responseBody = Serialize(new Response { transaction = new Transaction { id = transactionId }, completed = true });
            SetBasicResponseHeaders();
            return Task.CompletedTask;
        }

        private string GetTransactionKind()
        {
            if (RequestUri.AbsolutePath.Contains("weekday_stamina_recovery"))
                return "weekday_stamina_recovery";
            if (RequestUri.AbsolutePath.Contains("stamina_recovery"))
                return "stamina_recovery";
            if (RequestUri.AbsolutePath.Contains("enhancement_item_capacity"))
                return "enhancement_item_capacity";
            return "unknown";
        }

        private static bool DeductBillingPoint(PlayerProfile player, int amount)
        {
            if (Config.Get().InGame.InfiniteItems)
                return true;

            if (player.data.paidBlessing + player.data.freeBlessing < amount)
                return false;

            int paidDeduction = Math.Min(player.data.paidBlessing, amount);
            player.data.paidBlessing -= paidDeduction;

            int remaining = amount - paidDeduction;
            if (remaining > 0)
                player.data.freeBlessing -= remaining;

            return true;
        }

        public class Response
        {
            public Transaction transaction { get; set; } = new();
            public bool completed { get; set; }
        }

        public class Transaction
        {
            public long id { get; set; }
        }
    }
}
