namespace Yuyuyui.PrivateServer;

public class ExchangeItemCreateEntity : BaseEntity<ExchangeItemCreateEntity>
{
    public ExchangeItemCreateEntity(
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
        public Result exchange { get; set; } = new();

        public class Result
        {
            public CreateData product { get; set; } = new();

            public class CreateData
            {
                public long id { get; set; }
                public int before_count { get; set; } = 1;
            }
        }
    }
}