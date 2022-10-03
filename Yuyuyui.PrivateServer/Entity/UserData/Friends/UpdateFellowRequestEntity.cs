using System.Text;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class UpdateFellowRequestEntity : BaseEntity<UpdateFellowRequestEntity>
    {
        public UpdateFellowRequestEntity(
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
            //var player = GetPlayerFromCookies();
            
            long requestID = long.Parse(GetPathParameter("request_id"));
            FriendRequest friendRequest = FriendRequest.Load(requestID);

            Request requestObj = Deserialize<Request>(requestBody)!;
            
            // Update the friend request
            friendRequest.status = requestObj.fellow_request.status;
            friendRequest.ProcessStatus(); // should ultimately delete the request file

            Response responseObj;
            using (var cardsDb = new CardsContext())
            using (var charactersDb = new CharactersContext())
            {
                responseObj = new()
                {
                    fellow_request =
                        FellowRequestEntity.Response.Data.FromFriendRequest(cardsDb, charactersDb, friendRequest)
                };
            }
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Request
        {
            public long id { get; set; }
            public FellowRequestStatus fellow_request { get; set; } = new FellowRequestStatus();

            public class FellowRequestStatus
            {
                public int status { get; set; }
            }
        }

        public class Response
        {
            public FellowRequestEntity.Response.Data fellow_request { get; set; } = new();
        }
    }
}