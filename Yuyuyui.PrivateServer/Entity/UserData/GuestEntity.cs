using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer;

public class GuestEntity : BaseEntity<GuestEntity>
{
    public GuestEntity(
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
        // PlayerProfile player = await GetPlayerFromCookies();

        var dummyPlayer = await PrivateServer.EnsureDummyPlayer();

        var responseObj = new Response
        {
            supporters = new Dictionary<long, Response.SupporterData>
            {
                { long.Parse(dummyPlayer!.id.code), await Response.SupporterData.FromPlayer(dummyPlayer) },
                //{ long.Parse(player.id.code), await Response.SupporterData.FromPlayer(player) },
            }
        };

        responseBody = Serialize(responseObj);
        SetBasicResponseHeaders();
    }

    public class Response
    {
        public IDictionary<long, SupporterData> supporters { get; set; } = new Dictionary<long, SupporterData>();

        public class SupporterData
        {
            public long id { get; set; }
            public int level { get; set; }
            public string nickname { get; set; } = "";
            public long accessed_at { get; set; }
            public bool fellow { get; set; }
            public int friend_point { get; set; }
            public string user_id { get; set; } = "";
            public long? title_item_id { get; set; } = null;
            public CardDataWithSupport leader_card { get; set; } = new();

            public class CardData
            {
                public long hit_point { get; set; } = 0;
                public int attack { get; set; } = 0;
                public long user_card_id { get; set; } = 0;
                public long master_id { get; set; } = 0;
                public int potential { get; set; } = 0;
                public int evolution_level { get; set; } = 0;
                public int level { get; set; } = 0;

                public async Task UpdateWithUserCardId(long? cardId)
                {
                    if (cardId == null) return;
                    if (!await Card.Exists((long)cardId)) return;
                    var userCard = await Card.Load((long)cardId);

                    hit_point = userCard.GetHitPoint();
                    attack = userCard.GetAttack();
                    user_card_id = userCard.id;
                    master_id = userCard.master_id;
                    potential = userCard.potential;
                    evolution_level = userCard.evolution_level;
                    level = userCard.level;
                }

                public static async Task<CardData?> FromUserCardId(long? cardId)
                {
                    if (cardId == null) return null;
                    if (!await Card.Exists((long)cardId)) return null;

                    var data = new CardData();
                    await data.UpdateWithUserCardId((long)cardId);
                    return data;
                }
            }

            public class CardDataWithSupport : CardData
            {
                public long id { get; set; }
                public CardData? support { get; set; } = new();
                public CardData? support_2 { get; set; } = new();
                public CardData? assist { get; set; } = new();
                public List<AccessoryListEntity.Response.Accessory> accessories { get; set; } = [];

                public static async Task<CardDataWithSupport> FromDeck(Deck deck)
                {
                    Unit leaderUnit = await Unit.Load(deck.leaderUnitID);
                    var leaderUnitAccessories = await leaderUnit.accessories
                        .Select(Accessory.Load)
                        .WhenAll();
                    var data = new CardDataWithSupport()
                    {
                        id = leaderUnit.id,
                        support = await FromUserCardId(leaderUnit.supportCardID),
                        support_2 = await FromUserCardId(leaderUnit.supportCard2ID),
                        assist = await FromUserCardId(leaderUnit.assistCardID),
                        accessories = leaderUnitAccessories
                            .Select(AccessoryListEntity.Response.Accessory.FromPlayerAccessory)
                            .ToList()
                    };
                    await data.UpdateWithUserCardId(leaderUnit.baseCardID);

                    return data;
                }
            }

            public static async Task<SupporterData> FromPlayer(PlayerProfile player)
            {
                return new()
                {
                    id = long.Parse(player.id.code),
                    level = player.data.level,
                    nickname = player.profile.nickname,
                    accessed_at = player.data.lastActive,
                    fellow = false, // TODO
                    friend_point = 20, // TODO
                    user_id = player.id.code,
                    title_item_id = player.data.titleItemID,
                    leader_card = await CardDataWithSupport.FromDeck(await Deck.Load(player.decks[0]))
                };
            }
        }
    }
}