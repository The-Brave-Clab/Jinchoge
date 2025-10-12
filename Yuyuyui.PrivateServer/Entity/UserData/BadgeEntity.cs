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

        protected override async Task ProcessRequest()
        {
            var playerId = await GetPlayerIdFromCookies();

            if (requestBody.Length > 0)
            {
                // Utils.LogWarning("PUT Method, Needs more tests!");
                Request request = Deserialize<Request>(requestBody)!;

                await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
                {
                    var player = await PlayerProfile.Load(playerId.code);
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

                    await player.Save();
                }

                responseBody = "{}"u8.ToArray();
            }
            else
            {
                // Utils.LogWarning("Stub API!");
                IList<long> playerReceivedGifts;
                IList<long> playerFriendRequests;
                IDictionary<int, IList<int>> playerNewAlbum;
                await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
                {
                    var player = await PlayerProfile.Load(playerId.code);
                    playerReceivedGifts = player.receivedGifts;
                    playerFriendRequests = player.friendRequests;
                    playerNewAlbum = player.newAlbum;
                }

                Response responseObj = new()
                {
                    badge = new()
                    {
                        has_complete_mission = false, // update automatically?
                        has_complete_daily_mission = false, // update automatically?
                        has_present = playerReceivedGifts.Count > 0,
                        has_fellow_request = playerFriendRequests.Count > 0,
                        has_complete_club_working = false, // update automatically?
                        end_at_exchange = Utils.CurrentUnixTime() - 1, // taisha shop rewards end at this timestamp
                        has_exchangeable_bingo = false, // update automatically?
                        end_at_event = Utils.CurrentUnixTime() + 86400, // event items in taisha shop end at this timestamp
                        end_at_playback_event = Utils.CurrentUnixTime() - 1, // remastered event items ...
                        new_title = 0, // if the player has new title
                        new_album_categories = playerNewAlbum, // album { category_id : [ sub_category_id ] }
                        end_at_collab_event = Utils.CurrentUnixTime() - 1 // collaboration event items ...
                    }
                };

                responseBody = Serialize(responseObj);
            }
            
            SetBasicResponseHeaders();
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