using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class ClubWorkingOrderEntity : BaseEntity<ClubWorkingOrderEntity>
    {
        public ClubWorkingOrderEntity(
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

            // Utils.LogWarning("Reward boxes of club orders not filled!");

            Response responseObj;
            if (Config.Get().InGame.InfiniteItems)
            {
                using var clubWorkingsDb = new ClubWorkingsContext();
                responseObj = new()
                {
                    club_orders = clubWorkingsDb.ClubOrders
                        .ToList()
                        .Select(o => new Response.ClubOrderWithReward
                            {
                                id = o.Id,
                                master_id = o.Id,
                                quantity = 999,
                                reward_boxes = new List<long?>
                                    {
                                        o.RewardBox1Id,
                                        o.RewardBox2Id,
                                        o.RewardBox3Id
                                    }
                                    .Where(id => id != null)
                                    .Select(id => (long)id!)
                                    .Select(id => clubWorkingsDb.ClubOrderRewardBoxes.ToList().FirstOrDefault(box => box.Id == id))
                                    .Where(box => box != null)
                                    .Select(box => new ClubOrder.RewardBox
                                    {
                                        id = box!.Id,
                                        title = box!.Title
                                    })
                                    .ToList()
                            })
                        .ToList()
                };
            }
            else
            {
                responseObj = new()
                {
                    club_orders = player.clubOrders
                        .Select(l =>
                        {
                            ClubOrder order = ClubOrder.Load(l);
                            return new Response.ClubOrderWithReward
                            {
                                id = order.id,
                                master_id = order.master_id,
                                quantity = order.quantity,
                                reward_boxes = new List<ClubOrder.RewardBox>()
                            };
                        }).ToList()
                };
            }

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IList<ClubOrderWithReward> club_orders { get; set; } = new List<ClubOrderWithReward>();

            public class ClubOrderWithReward
            {
                public long id { get; set; }
                public long master_id { get; set; }
                public int quantity { get; set; }
                public IList<ClubOrder.RewardBox> reward_boxes { get; set; } = new List<ClubOrder.RewardBox>();
            }
        }
    }
}