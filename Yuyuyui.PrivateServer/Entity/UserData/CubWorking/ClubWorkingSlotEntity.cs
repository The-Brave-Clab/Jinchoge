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
            RouteConfig config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override async Task ProcessRequest()
        {
            var playerId = await GetPlayerIdFromCookies();

            IList<long> playerSlots;
            await using (await IDistributedLockProvider.ActiveProvider!.AcquirePlayerProfileLock(playerId.code))
            {
                var player = await PlayerProfile.Load(playerId.code);

                if (player.clubWorkingSlots.Count == 0)
                {
                    player.clubWorkingSlots = new List<long>(3);
                    for (int i = 0; i < 3; ++i)
                    {
                        var newSlot = await ClubWorkingSlot.NewEmptySlot();
                        player.clubWorkingSlots.Add(newSlot.id);
                        await newSlot.Save();
                    }

                    await player.Save();
                }

                // Utils.LogWarning("Stub API! Process finished club working here!");
                playerSlots = player.clubWorkingSlots;
            }

            Response responseObj = new()
            {
                club_working_slots = (await ClubWorkingSlot.LoadMany(playerSlots)).ToList()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }

        public class Response
        {
            public IList<ClubWorkingSlot> club_working_slots { get; set; } = new List<ClubWorkingSlot>();
        }
    }
}