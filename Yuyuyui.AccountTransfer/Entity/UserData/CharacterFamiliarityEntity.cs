using System;
using Titanium.Web.Proxy.Http;
using Yuyuyui.PrivateServer;

namespace Yuyuyui.AccountTransfer
{
    public class CharacterFamiliarityEntity : BaseEntity<CharacterFamiliarityEntity>
    {
        public CharacterFamiliarityEntity(
            Uri requestUri,
            string httpMethod,
            Config config)
            : base(requestUri, httpMethod, config)
        {
        }

        public override void ProcessRequest(byte[] requestBody, HeaderCollection requestHeaders,
            ref AccountTransferProxyCallbacks.PlayerSession playerSession)
        {
        }

        public override void ProcessResponse(byte[] responseBody, HeaderCollection responseHeaders,
            ref AccountTransferProxyCallbacks.PlayerSession playerSession)
        {
            var response = Deserialize<PrivateServer.CharacterFamiliarityEntity.Response>(responseBody)!;

            playerSession.player!.characterFamiliarities = response.character_familiarities;
            playerSession.player!.Save();
            
            Utils.LogTrace($"Got character familiarities.");
        }
    }
}