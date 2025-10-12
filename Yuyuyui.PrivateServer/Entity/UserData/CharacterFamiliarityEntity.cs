using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class CharacterFamiliarityEntity : BaseEntity<CharacterFamiliarityEntity>
    {
        public CharacterFamiliarityEntity(
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

            IDictionary<string, CharacterFamiliarityWithAssist> playerCharacterFamiliarities;
            await using (await PrivateServer.ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(playerId.code))
            {
                var player = await PlayerProfile.Load(playerId.code);
                playerCharacterFamiliarities = player.characterFamiliarities;
            }

            Response responseObj = new()
            {
                character_familiarities = playerCharacterFamiliarities
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();
        }

        public class Response
        {
            public IDictionary<string, CharacterFamiliarityWithAssist> character_familiarities { get; set; } =
                new Dictionary<string, CharacterFamiliarityWithAssist>();
        }
    }
}