using System.Threading.Tasks;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer;

public class FriendRequest : BasePlayerData<FriendRequest, long>
{
    public override long Identifier => id;

    public long id { get; set; }
    public int status { get; set; } // Requested = 0, Accepted = 1, Rejected = 2
    public long createdAt { get; set; }
    public string fromUser { get; set; } = "";
    public string toUser { get; set; } = "";

    private static async Task<long> GetID()
    {
        long new_id = long.Parse(Utils.GenerateRandomDigit(9));
        while (await Exists(new_id))
        {
            new_id = long.Parse(Utils.GenerateRandomDigit(9));
        }

        return new_id;
    }

    public static async Task<FriendRequest> CreateOrLoad(PlayerProfile.ID fromId, PlayerProfile to)
    {
        // first, find if the request exists
        // if exists, do nothing but return the existing request
        foreach (var requestID in to.friendRequests)
        {
            FriendRequest req = await Load(requestID);
            if (req.fromUser == fromId.code)
            {
                Utils.LogTrace(string.Format(Resources.LOG_PS_FRIEND_REQUEST_FOUND, req.id, fromId.code, to.id.code));
                return req;
            }
        }
            
        // if it doesn't exist, create it
        FriendRequest request = new FriendRequest
        {
            id = await GetID(),
            status = 0,
            createdAt = Utils.CurrentUnixTime(),
            fromUser = fromId.code,
            toUser = to.id.code
        };
        await request.Save();
        Utils.LogTrace(string.Format(Resources.LOG_PS_FRIEND_REQUEST_CREATED, request.id, fromId.code, to.id.code));

        // and save into the requested user's profile
        if (!to.id.code.StartsWith("0"))
        {
            to.friendRequests.Add(request.id);
            await to.Save();
        }
        Utils.Log(string.Format(Resources.LOG_PS_FRIEND_REQUEST_SENT, request.id, to.id.code));

        return request;
    }

    private async Task Remove()
    {
        // delete the id from target player
        PlayerProfile toPlayer = await PlayerProfile.Load(toUser);
        toPlayer.friendRequests.Remove(id);
        await toPlayer.Save();
        Utils.Log(string.Format(Resources.LOG_PS_FRIEND_REQUEST_REMOVED, id, toPlayer.id.code));
            
        // delete the file
        await Delete();
        Utils.Log(string.Format(Resources.LOG_PS_FRIEND_REQUEST_DELETED, id, fromUser, toUser));
    }

    public async Task ProcessStatus()
    {
        if (status == 0)
        {
            Utils.LogError(Resources.LOG_PS_FRIEND_REQUEST_UNKNOWN);
            return;
        }

        if (status == 1) // accepted
        {
            // add friend
            PlayerProfile from = await PlayerProfile.Load(fromUser);
            PlayerProfile to = await PlayerProfile.Load(toUser);
                
            from.friends.Add(to.id.code);
            await from.Save();
                
            if (!to.id.code.StartsWith("0"))
            {
                to.friends.Add(from.id.code);
                await to.Save();
            }
                
            Utils.Log(string.Format(Resources.LOG_PS_FRIEND_REQUEST_ACCEPTED, from.id.code, to.id.code));
        }
        else if (status == 2) // rejected
        {
            // does nothing
            Utils.Log(string.Format(Resources.LOG_PS_FRIEND_REQUEST_REJECTED, toUser, fromUser));
        }

        if (status is 1 or 2) // huh good looking syntax
        {
            // finished processing, remove the request from "database"
            await Remove();
            return;
        }
            
        Utils.LogError(Resources.LOG_PS_FRIEND_REQUEST_UNKNOWN);
    }
}