using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class BadgeEntity : BaseEntity<BadgeEntity>
    {
        public BadgeEntity(
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


            if (requestBody.Length > 0)
            {
                // Utils.LogWarning("PUT Method, Needs more tests!");
                Request request = Deserialize<Request>(requestBody)!;
                if (request.sub_category_id == -1)
                {
                    player.newAlbum.Remove(request.category_id);
                    // Utils.Log(
                    //     $"Updated user new album status:\n\tCategory\t{request.category_id}");
                }
                else
                {
                    player.newAlbum[request.category_id].Remove(request.sub_category_id);
                    if (player.newAlbum[request.category_id].Count == 0)
                        player.newAlbum.Remove(request.category_id);
                    // Utils.Log(
                    //     $"Updated user new album status:\n\tCategory\t{request.category_id}\n\tSubcategory\t{request.sub_category_id}");
                }
                player.Save();
                
                responseBody = Encoding.UTF8.GetBytes("{}");
            }
            else
            {
                // Utils.LogWarning("Stub API!");
                
                Response responseObj = new()
                {
                    badge = new()
                    {
                        has_complete_mission = true, // update automatically?
                        has_complete_daily_mission = true, // update automatically?
                        has_present = player.receivedGifts.Count > 0,
                        has_fellow_request = player.friendRequests.Count > 0,
                        has_complete_club_working = true, // update automatically?
                        end_at_exchange = Utils.CurrentUnixTime() + 120, // ?
                        has_exchangeable_bingo = true, // update automatically?
                        end_at_event = Utils.CurrentUnixTime() + 180, // ?
                        end_at_playback_event = Utils.CurrentUnixTime() + 240, // ?
                        new_title = 1, // ?
                        new_album_categories = player.newAlbum, // album { category_id : [ sub_category_id ] }
                        end_at_collab_event = Utils.CurrentUnixTime() + 320 // ?
                    }
                };

                responseBody = Serialize(responseObj);
            }
            
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Request
        {
            public int category_id { get; set; }
            public int sub_category_id { get; set; }
        }

        public class Response
        {
            public BadgeData badge { get; set; } = new();

            public class BadgeData
            {
                public bool has_complete_mission { get; set; }
                public bool has_complete_daily_mission { get; set; }
                public bool has_present { get; set; }
                public bool has_fellow_request { get; set; }
                public bool has_complete_club_working { get; set; }
                public long? end_at_exchange { get; set; } = null; // unixtime
                public bool has_exchangeable_bingo { get; set; }
                public long? end_at_event { get; set; } = null; // assumption
                public long? end_at_playback_event { get; set; } = null; // assumption
                public int new_title { get; set; } // unknown

                public IDictionary<int, IList<int>> new_album_categories { get; set; } =
                    new Dictionary<int, IList<int>>(); // category_id, sub_category_id

                public long? end_at_collab_event { get; set; } = null; // assumption
            }
        }
    }
}