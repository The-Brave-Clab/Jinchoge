namespace Yuyuyui.PrivateServer
{
    public class FriendRequest : BasePlayerData<FriendRequest, long>
    {
        protected override long Identifier => id;

        public long id { get; set; }
        public int status { get; set; } // Requested = 0, Accepted = 1, Rejected = 2
        public long createdAt { get; set; }
        public string fromUser { get; set; } = "";
        public string toUser { get; set; } = "";

        private static long GetID()
        {
            long new_id = long.Parse(Utils.GenerateRandomDigit(9));
            while (Exists(new_id))
            {
                new_id = long.Parse(Utils.GenerateRandomDigit(9));
            }

            return new_id;
        }

        public static FriendRequest CreateOrLoad(PlayerProfile from, PlayerProfile to)
        {
            // first, find if the request exists
            // if exists, do nothing but return the existing request
            foreach (var requestID in to.friendRequests)
            {
                FriendRequest req = Load(requestID);
                if (req.fromUser == from.id.code)
                {
                    Utils.Log($"Found an existed friend request {req.id} ({from.id.code}=>{to.id.code})");
                    return req;
                }
            }
            
            // if it doesn't exist, create it
            FriendRequest request = new FriendRequest
            {
                id = GetID(),
                status = 0,
                createdAt = Utils.CurrentUnixTime(),
                fromUser = from.id.code,
                toUser = to.id.code
            };
            request.Save();
            Utils.Log($"Created friend request {request.id} ({from.id.code}=>{to.id.code})");

            // and save into the requested user's profile
            to.friendRequests.Add(request.id);
            to.Save();
            Utils.Log($"Friend request {request.id} sent to player {to.id.code}");

            return request;
        }

        private void Remove()
        {
            // delete the id from target player
            PlayerProfile toPlayer = PlayerProfile.Load(toUser);
            toPlayer.friendRequests.Remove(id);
            toPlayer.Save();
            Utils.Log($"Friend request {id} removed from player {toPlayer.id.code}");
            
            // delete the file
            DeleteFile();
            Utils.Log($"Friend request {id} ({fromUser}=>{toUser}) deleted!");
        }

        public void ProcessStatus()
        {
            if (status == 0)
            {
                Utils.LogError("Process the status of a friend request, but the request is neither accepted nor rejected!");
                return;
            }

            if (status == 1) // accepted
            {
                // add friend
                PlayerProfile from = PlayerProfile.Load(fromUser);
                PlayerProfile to = PlayerProfile.Load(toUser);
                
                from.friends.Add(to.id.code);
                to.friends.Add(from.id.code);
                
                from.Save();
                to.Save();
                
                Utils.Log($"Player {from.id.code} and {to.id.code} are friends now!");
            }
            else if (status == 2) // rejected
            {
                // does nothing
                Utils.Log($"Player {toUser} rejected the friend request from {fromUser}");
            }

            if (status is 1 or 2) // huh good looking syntax
            {
                // finished processing, remove the request from "database"
                Remove();
                return;
            }
            
            Utils.LogError("Unknown status, shouldn't be here!");
        }
    }
}