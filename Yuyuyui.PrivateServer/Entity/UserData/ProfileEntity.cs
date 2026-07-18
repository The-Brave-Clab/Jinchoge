using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer
{
    public class ProfileEntity : BaseEntity<ProfileEntity>
    {
        public ProfileEntity(
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

            if (requestBody.Length > 0)
            {
                RequestResponse request = Deserialize<RequestResponse>(requestBody)!;
                if (request.profile != null)
                {
                    Utils.Log(string.Format(Resources.LOG_PS_PROFILE_UPDATED,
                        request.profile.nickname, request.profile.comment));
                    player.profile = request.profile;
                }

                ApplyLeaderDeckCard(player, request.leader_deck_card_id);
                player.Save();
            }

            RequestResponse responseObj = new RequestResponse
            {
                profile = player.profile,
                leader_deck_card_id = GetLeaderDeckCardId(player)
            };
            
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        private static void ApplyLeaderDeckCard(PlayerProfile player, long? leaderDeckCardId)
        {
            if (leaderDeckCardId == null || player.decks.Count == 0)
                return;

            Deck deck = Deck.Load(player.decks[0]);
            if (!deck.units.Contains(leaderDeckCardId.Value))
                return;

            deck.leaderUnitID = leaderDeckCardId.Value;
            deck.Save();
        }

        private static long? GetLeaderDeckCardId(PlayerProfile player)
        {
            if (player.decks.Count == 0)
                return null;

            return Deck.Load(player.decks[0]).leaderUnitID;
        }

        public class RequestResponse
        {
            public PlayerProfile.Profile? profile { get; set; } = null;
            public long? leader_deck_card_id { get; set; } = null;
        }
    }
}
