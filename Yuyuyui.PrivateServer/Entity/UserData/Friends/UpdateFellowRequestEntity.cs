using System.Text;

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
            var player = GetPlayerFromCookies();
            
            long requestID = long.Parse(GetPathParameter("request_id"));
            FriendRequest friendRequest = FriendRequest.Load(requestID);

            Request requestObj = Deserialize<Request>(requestBody)!;
            
            // Update the friend request
            friendRequest.status = requestObj.fellow_request.status;
            friendRequest.ProcessStatus(); // should ultimately delete the request file

            Response responseObj = new()
            {
                fellow_request = friendRequest
            };
            
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