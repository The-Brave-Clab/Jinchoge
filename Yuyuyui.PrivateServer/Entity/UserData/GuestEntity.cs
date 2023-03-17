using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        var responseObj = new Response
        {
            supporters = new Dictionary<long, Response.SupporterData>() // TODO
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
            public int id { get; set; }
            public int level { get; set; }
            public string nickname { get; set; } = "";
            public int accessed_at { get; set; }
            public bool fellow { get; set; }
            public int friend_point { get; set; }
            public string user_id { get; set; } = "";
            public CardDataWithSupport leader_card { get; set; } = new();

            public class CardData
            {
                public long hit_point { get; set; }
                public int attack { get; set; }
                public long user_card_id { get; set; }
                public long master_id { get; set; }
                public int potential { get; set; }
                public int evolution_level { get; set; }
                public int level { get; set; }
            }
            
            public class CardDataWithSupport : CardData
            {
                public long id { get; set; }
                public CardData? support { get; set; } = null;
                public CardData? support_2 { get; set; } = null;
                public CardData? assist { get; set; } = null;
                public List<AccessoryData> accessories { get; set; } = new();
            }

            public class AccessoryData
            {
                public long id { get; set; }
                public long master_id { get; set; }
                public int level { get; set; }
                public long hit_point { get; set; }
                public int attack { get; set; }
                public int cost { get; set; }
                public int money { get; set; }
                public int quantity { get; set; }
                public int next_quantity { get; set; }
            }
        }
    }
}