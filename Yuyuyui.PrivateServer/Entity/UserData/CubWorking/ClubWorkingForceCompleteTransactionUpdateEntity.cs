using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class ClubWorkingForceCompleteTransactionUpdateEntity : BaseEntity<ClubWorkingForceCompleteTransactionUpdateEntity>
    {
        public ClubWorkingForceCompleteTransactionUpdateEntity(
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
            EnsureClubWorkingSlots(player);
            long clubWorkingId = long.Parse(GetPathParameter("club_working_id"));
            long transactionId = long.Parse(GetPathParameter("transaction_id"));

            ClubWorkingSlot slot = player.clubWorkingSlots
                .Select(ClubWorkingSlot.Load)
                .FirstOrDefault(s => clubWorkingId != 0 && s.club_working_id == clubWorkingId)
                ?? player.clubWorkingSlots
                    .Select(ClubWorkingSlot.Load)
                    .FirstOrDefault(s => !s.available)
                ?? CreateAdditionalSlot(player);

            slot.finishment_time = Utils.CurrentUnixTime() - 1;
            slot.Save();

            Response responseObj = new()
            {
                transaction = new()
                {
                    id = transactionId
                },
                club_working = slot,
                club_working_slot = slot
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        private static void EnsureClubWorkingSlots(PlayerProfile player)
        {
            if (player.clubWorkingSlots.Count != 0) return;

            player.clubWorkingSlots = new List<long>(3);
            for (int i = 0; i < 3; ++i)
            {
                var newSlot = ClubWorkingSlot.NewEmptySlot();
                player.clubWorkingSlots.Add(newSlot.id);
                newSlot.Save();
            }

            player.Save();
        }

        private static ClubWorkingSlot CreateAdditionalSlot(PlayerProfile player)
        {
            var newSlot = ClubWorkingSlot.NewEmptySlot();
            player.clubWorkingSlots.Add(newSlot.id);
            newSlot.Save();
            player.Save();
            return newSlot;
        }

        public class Response
        {
            public Transaction transaction { get; set; } = new();
            public ClubWorkingSlot club_working { get; set; } = new();
            public ClubWorkingSlot club_working_slot { get; set; } = new();

            public class Transaction
            {
                public long id { get; set; }
            }
        }
    }
}
