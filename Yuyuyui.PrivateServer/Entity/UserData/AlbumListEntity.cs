using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer;

public class AlbumListEntity : BaseEntity<AlbumListEntity>
{
    public AlbumListEntity(
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

        PlayerProfile.Progress playerProgress;
        await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
        {
            var player = await PlayerProfile.Load(playerId.code);
            playerProgress = player.progress;
        }
            
        // Utils.LogWarning("All adventure books are unlocked and watchable for now!");
        // Utils.LogWarning("Player adventure books ticket count is fixed!");

        if (HasRequestBody())
        {
            // TODO: Unlock the requested adventure book
            PostRequest requestObj = Deserialize<PostRequest>(requestBody)!;

            PostResponse responseObj = new()
            {
                adventure_book = new()
                {
                    id = requestObj.adventure_book_id, // See the definition of id
                    master_id = requestObj.adventure_book_id,
                    watched = playerProgress.adventureBooksRead.Contains(requestObj.adventure_book_id)
                }
            };

            responseBody = Serialize(responseObj);
        }
        else
        {
            List<AdventureBook> watchable;
            List<AdventureBook> unwachable;
            using (AdventureBooksContext adventureBooksDb = new())
            {
                watchable = adventureBooksDb.AdventureBooks
                    .Where(b => true) // TODO
                    .ToList();
                unwachable = adventureBooksDb.AdventureBooks
                    .Where(b => false) // TODO
                    .ToList();
            }

            GetResponse responseObj = new()
            {
                watchable_adventure_books = watchable
                    .Select(b => new AdventureBookStatus
                    {
                        id = b.Id, // See the definition of id
                        master_id = b.Id,
                        watched = playerProgress.adventureBooksRead.Contains(b.Id)
                    })
                    .ToDictionary(s => s.id, s => s),
                unwatchable_adventure_books = unwachable
                    .Select(b => b.Id)
                    .ToList(),
                adventure_book_tickets = new List<GetResponse.AdventureBookTickets>
                {
                    new()
                    {
                        master_id = 1,
                        quantity = 200 // TODO
                    }
                }
            };

            responseBody = Serialize(responseObj);
        }
            
        SetBasicResponseHeaders();
    }

    public class AdventureBookStatus
    {
        public long id { get; set; } // This in theory should be a player data identifier,
        // but we really don't want to save thousands of files just for this,
        // so we use master_id instead, and associate it with the player
        // through the request cookies.
        public long master_id { get; set; }
        public bool watched { get; set; }
    }

    public class GetResponse
    {
        public IDictionary<long, AdventureBookStatus> watchable_adventure_books { get; set; } =
            new Dictionary<long, AdventureBookStatus>(); // key is AdventureBookStatus.id

        public IList<long> unwatchable_adventure_books { get; set; } = 
            new List<long>(); // AdventureBookStatus.master_id

        public IList<AdventureBookTickets> adventure_book_tickets = 
            new List<AdventureBookTickets>(); // TODO

        public class AdventureBookTickets
        {
            public long master_id { get; set; }
            public long quantity { get; set; }
        }
    }

    public class PostRequest
    {
        public long adventure_book_id { get; set; } // unlock this
    }

    public class PostResponse
    {
        public AdventureBookStatus adventure_book { get; set; } = new();
    }
}