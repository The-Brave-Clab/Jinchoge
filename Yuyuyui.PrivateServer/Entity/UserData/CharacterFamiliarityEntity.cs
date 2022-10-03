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
            Config config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();

            Response responseObj = new()
            {
                character_familiarities = player.characterFamiliarities
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IDictionary<string, CharacterFamiliarity> character_familiarities { get; set; } =
                new Dictionary<string, CharacterFamiliarity>();
        }
    }
}