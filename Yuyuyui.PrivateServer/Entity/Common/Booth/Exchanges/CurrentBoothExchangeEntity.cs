namespace Yuyuyui.PrivateServer;

public class CurrentBoothExchangeEntity : BaseEntity<CurrentBoothExchangeEntity>
{
    public CurrentBoothExchangeEntity(
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
        // var player = GetPlayerFromCookies();

        Response response = new()
        {
            exchange = new()
            {
                product = new()
                {
                    id = long.Parse(pathParameters["exchange_item"]),
                    before_count = 1
                }
            }
        };

        responseBody = Serialize(response);
        
        SetBasicResponseHeaders();
        return Task.CompletedTask;
    }

    public class Response
    {
        public CurrentExchange exchange { get; set; }

        public class CurrentExchange
        {
            public CurrentProduct product { get; set; }

            public class CurrentProduct
            {
                public long id { get; set; }
                public int before_count { get; set; } = 1;
            }
        }
    }
}