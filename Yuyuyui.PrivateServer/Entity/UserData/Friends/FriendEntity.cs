using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class FriendEntity : BaseEntity<FriendEntity>
    {
        public FriendEntity(
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

            Response responseObj;
            using (var cardsDb = new CardsContext())
            using (var charactersDb = new CharactersContext())
            {
                responseObj = new()
                {
                    fellowships = player.friends
                        .Select(PlayerProfile.Load)
                        .ToDictionary(p => p.id.code,
                            p => UserInfoEntity.Response.User.FromPlayerProfile(cardsDb, charactersDb, p))
                };
            }
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IDictionary<string, UserInfoEntity.Response.User> fellowships { get; set; } = 
                new Dictionary<string, UserInfoEntity.Response.User>();
        }
    }
}