using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class ClubWorkingSlotEntity : BaseEntity<ClubWorkingSlotEntity>
    {
        public ClubWorkingSlotEntity(
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

            if (player.clubWorkingSlots.Count == 0)
            {
                player.clubWorkingSlots = new List<long>(3);
                for (int i = 0; i < 3; ++i)
                {
                    var newSlot = ClubWorkingSlot.NewEmptySlot();
                    player.clubWorkingSlots.Add(newSlot.id);
                    newSlot.Save();
                }
                player.Save();
            }

            Utils.LogWarning("Stub API! Process finished club working here!");

            Response responseObj = new()
            {
                club_working_slots = player.clubWorkingSlots
                    .Select(ClubWorkingSlot.Load)
                    .ToList()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IList<ClubWorkingSlot> club_working_slots { get; set; } = new List<ClubWorkingSlot>();
        }
    }
}