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

    protected override Task ProcessRequest()
    {
        PlayerProfile player = GetPlayerFromCookies();

        var dummyPlayer = PrivateServer.EnsureDummyPlayer();

        using var cardsDb = new CardsContext();
        using var accessoriesDb = new AccessoriesContext();
        var responseObj = new Response
        {
            supporters = new Dictionary<long, Response.SupporterData>
            {
                { long.Parse(dummyPlayer.id.code), Response.SupporterData.FromPlayer(cardsDb, accessoriesDb, dummyPlayer) },
                //{ long.Parse(player.id.code), Response.SupporterData.FromPlayer(cardsDb, accessoriesDb, player) },
            }
        };

        responseBody = Serialize(responseObj);
        SetBasicResponseHeaders();

        return Task.CompletedTask;
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

                public void UpdateWithUserCardId(CardsContext cardsDb, long? cardId)
                {
                    if (cardId == null) return;
                    if (!Card.Exists((long)cardId)) return;
                    var userCard = Card.Load((long)cardId);

                    hit_point = userCard.GetHitPoint(cardsDb);
                    attack = userCard.GetAttack(cardsDb);
                    user_card_id = userCard.id;
                    master_id = userCard.master_id;
                    potential = userCard.potential;
                    evolution_level = userCard.evolution_level;
                    level = userCard.level;
                }

                public static object FromUserCardId(CardsContext cardsDb, long? cardId)
                {
                    if (cardId == null) return new();
                    if (!Card.Exists((long)cardId)) return new();

                    var data = new CardData();
                    data.UpdateWithUserCardId(cardsDb, (long)cardId);
                    return data;
                }
            }

            public class CardDataWithSupport : CardData
            {
                public long id { get; set; }
                public object support { get; set; } = new();
                public object support_2 { get; set; } = new();
                public object assist { get; set; } = new();
                public List<AccessoryListEntity.Response.Accessory> accessories { get; set; } = new();

                public static CardDataWithSupport FromDeck(CardsContext cardsDb, AccessoriesContext accessoriesDb,
                    Deck deck)
                {
                    Unit leaderUnit = Unit.Load(deck.leaderUnitID);
                    var data = new CardDataWithSupport()
                    {
                        id = leaderUnit.id,
                        support = FromUserCardId(cardsDb, leaderUnit.supportCardID),
                        support_2 = FromUserCardId(cardsDb, leaderUnit.supportCard2ID),
                        assist = FromUserCardId(cardsDb, leaderUnit.assistCardID),
                        accessories = leaderUnit.accessories
                            .Select(a =>
                                AccessoryListEntity.Response.Accessory.FromPlayerAccessory(accessoriesDb,
                                    Accessory.Load(a)))
                            .ToList()
                    };
                    data.UpdateWithUserCardId(cardsDb, leaderUnit.baseCardID);

                    return data;
                }
            }

            public static SupporterData FromPlayer(CardsContext cardsDb, AccessoriesContext accessoriesDb,
                PlayerProfile player)
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
                    leader_card = CardDataWithSupport.FromDeck(cardsDb, accessoriesDb, Deck.Load(player.decks[0]))
                };
            }
        }
    }
}