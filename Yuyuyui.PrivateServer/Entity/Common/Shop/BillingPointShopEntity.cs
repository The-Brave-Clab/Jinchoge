namespace Yuyuyui.PrivateServer
{
    public class BillingPointShopEntity : BaseEntity<BillingPointShopEntity>
    {
        public BillingPointShopEntity(
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

            Utils.LogWarning("Stub API, package_items is empty for now");

            Response responseObj = new()
            {
                billing_point_shop = new()
                {
                    enhancement_item_capacity = new()
                    {
                        point_amount = 30,
                        added_capacity = 10,
                        max_capacity = 730
                    },
                    stamina_recovery = new()
                    {
                        point_amount = 50
                    },
                    weekday_stamina_recovery = new()
                    {
                        point_amount = 50
                    },
                    package_items = new()
                    {
                        products = new Dictionary<string, Response.Product>() // empty for now
                    }
                }
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public Body billing_point_shop { get; set; } = new();

            public class Body
            {
                public EnhancementItemCapacity enhancement_item_capacity { get; set; } = new();
                public BaseData stamina_recovery { get; set; } = new();
                public BaseData weekday_stamina_recovery { get; set; } = new();
                public PackageItem package_items { get; set; } = new(); // This could be a dictionary.
            }

            public class EnhancementItemCapacity : BaseData
            {
                public int added_capacity { get; set; }
                public int max_capacity { get; set; }
            }

            public class BaseData
            {
                public int point_amount { get; set; }
            }

            public class PackageItem
            {
                public IDictionary<string, Product> products { get; set; } = new Dictionary<string, Product>();
            }

            public class Product
            {
                public long id { get; set; }
                public long item_master_id { get; set; }
                public int item_category_id { get; set; }
                public int order { get; set; }
                public int consumption_resource_id { get; set; }
                public int point_amount { get; set; }
                public int purchased_quantity { get; set; }
                public int purchase_limit_quantity { get; set; }
                public string info_url { get; set; } = "";
                public long end_at { get; set; } // unixtime
                public string description { get; set; } = "";
                public string alert_message { get; set; } = "";
                public IList<Item> items = new List<Item>();

                public class Item
                {
                    public long item_master_id { get; set; }
                    public int item_category_id { get; set; }
                    public int quantity { get; set; }
                    public string name { get; set; } = "";
                }
            }
        }
    }
}