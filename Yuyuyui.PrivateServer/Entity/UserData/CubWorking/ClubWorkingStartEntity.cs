using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class ClubWorkingStartEntity : BaseEntity<ClubWorkingStartEntity>
    {
        public ClubWorkingStartEntity(
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
            Request request = Deserialize<Request>(requestBody)!;

            ClubWorkingSlot slot = player.clubWorkingSlots
                .Select(ClubWorkingSlot.Load)
                .FirstOrDefault(s => s.id == request.club_working_slot_id)
                ?? player.clubWorkingSlots
                    .Select(ClubWorkingSlot.Load)
                    .FirstOrDefault(s => s.available)
                ?? CreateAdditionalSlot(player);

            slot.available = false;
            slot.club_working_id = GenerateWorkingId(player);
            slot.primary_user_card_id = request.primary_user_card_id;
            slot.secondary_user_card_id = request.secondary_user_card_id;
            slot.club_order_master_id = request.club_order_master_id;
            slot.finishment_time = Utils.CurrentUnixTime() - 1;
            slot.Save();

            Response responseObj = new()
            {
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

        private static long GenerateWorkingId(PlayerProfile player)
        {
            long newId = long.Parse(Utils.GenerateRandomDigit(9));
            while (player.clubWorkingSlots
                       .Select(ClubWorkingSlot.Load)
                       .Any(s => s.club_working_id == newId))
                newId = long.Parse(Utils.GenerateRandomDigit(9));

            return newId;
        }

        public class Request
        {
            public long club_working_slot_id { get; set; }
            public int club_order_master_id { get; set; }
            public long? primary_user_card_id { get; set; }
            public long? secondary_user_card_id { get; set; }
        }

        public class Response
        {
            public ClubWorkingSlot club_working { get; set; } = new();
            public ClubWorkingSlot club_working_slot { get; set; } = new();
        }
    }
}
