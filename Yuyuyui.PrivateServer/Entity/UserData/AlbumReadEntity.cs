using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class AlbumReadEntity : BaseEntity<AlbumReadEntity>
    {
        public AlbumReadEntity(
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

            Request requestObj = Deserialize<Request>(requestBody)!;

            await using (await IDistributedLockProvider.ActiveProvider!.AcquirePlayerProfileLock(playerId.code))
            {
                var player = await PlayerProfile.Load(playerId.code);

                if (!player.progress.adventureBooksRead.Contains(requestObj.id))
                {
                    player.progress.adventureBooksRead.Add(requestObj.id);
                    await player.Save();
                }
            }

            AlbumListEntity.PostResponse responseObj = new()
            {
                adventure_book = new()
                {
                    id = requestObj.id, // See the definition of id
                    master_id = requestObj.id,
                    watched = true // Has to be true
                }
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }

        public class Request
        {
            public long id { get; set; } // See definition of AlbumListEntity.AdventureBookStatus.id
        }
    }
}