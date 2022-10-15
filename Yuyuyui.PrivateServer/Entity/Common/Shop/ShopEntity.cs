using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class ShopEntity : BaseEntity<ShopEntity>
    {
        public ShopEntity(
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

            Utils.LogWarning("Stub API, fixed items for now");

            Response responseObj = new()
            {
                shops = new List<Response.ShopData>
                {
                    new()
                    {
                        id = "C3365",
                        start_at = Utils.CurrentUnixTime() - 10,
                        end_at = Utils.CurrentUnixTime() + 290,
                        products =
                        {
                            {
                                "10093", new()
                                {
                                    id = 10093,
                                    item_master_id = 100040,
                                    item_category_id = 1,
                                    order = 1,
                                    purchased_quantity = 0,
                                    purchase_limit_quantity = 5,
                                    consumption_resource_id = 4,
                                    min_price = 50000,
                                    max_price = 150000,
                                    growth_kind = 3
                                }
                            },
                            {
                                "10094", new()
                                {
                                    id = 10094,
                                    item_master_id = 100140,
                                    item_category_id = 1,
                                    order = 2,
                                    purchased_quantity = 0,
                                    purchase_limit_quantity = 5,
                                    consumption_resource_id = 4,
                                    min_price = 50000,
                                    max_price = 150000,
                                    growth_kind = 3
                                }
                            },
                            {
                                "10095", new()
                                {
                                    id = 10095,
                                    item_master_id = 100140,
                                    item_category_id = 1,
                                    order = 3,
                                    purchased_quantity = 0,
                                    purchase_limit_quantity = 5,
                                    consumption_resource_id = 8,
                                    min_price = 5,
                                    max_price = 9,
                                    growth_kind = 3
                                }
                            }
                        }
                    },
                    new()
                    {
                        id = "A3365",
                        start_at = Utils.CurrentUnixTime() - 10,
                        end_at = Utils.CurrentUnixTime() + 290,
                        products =
                        {
                            {
                                "10093", new()
                                {
                                    id = 10093,
                                    item_master_id = 500101,
                                    item_category_id = 2,
                                    order = 1,
                                    purchased_quantity = 0,
                                    purchase_limit_quantity = 20,
                                    consumption_resource_id = 4,
                                    min_price = 6000,
                                    max_price = 63000,
                                    growth_kind = 3
                                }
                            },
                            {
                                "10094", new()
                                {
                                    id = 10094,
                                    item_master_id = 500109,
                                    item_category_id = 2,
                                    order = 2,
                                    purchased_quantity = 0,
                                    purchase_limit_quantity = 20,
                                    consumption_resource_id = 4,
                                    min_price = 6000,
                                    max_price = 63000,
                                    growth_kind = 3
                                }
                            },
                            {
                                "10095", new()
                                {
                                    id = 10095,
                                    item_master_id = 500091,
                                    item_category_id = 2,
                                    order = 3,
                                    purchased_quantity = 0,
                                    purchase_limit_quantity = 20,
                                    consumption_resource_id = 4,
                                    min_price = 8000,
                                    max_price = 84000,
                                    growth_kind = 3
                                }
                            }
                        }
                    },
                    new()
                    {
                        id = "I3365",
                        start_at = Utils.CurrentUnixTime() - 10,
                        end_at = Utils.CurrentUnixTime() + 290,
                        products =
                        {
                            {
                                "10093", new()
                                {
                                    id = 10093,
                                    item_master_id = 2,
                                    item_category_id = 3,
                                    order = 1,
                                    purchased_quantity = 0,
                                    purchase_limit_quantity = 20,
                                    consumption_resource_id = 4,
                                    min_price = 1000,
                                    max_price = 5750,
                                    growth_kind = 3
                                }
                            },
                            {
                                "10094", new()
                                {
                                    id = 10094,
                                    item_master_id = 10310,
                                    item_category_id = 4,
                                    order = 2,
                                    purchased_quantity = 0,
                                    purchase_limit_quantity = 10,
                                    consumption_resource_id = 4,
                                    min_price = 1000,
                                    max_price = 5500,
                                    growth_kind = 3
                                }
                            },
                            {
                                "10095", new()
                                {
                                    id = 10095,
                                    item_master_id = 10410,
                                    item_category_id = 4,
                                    order = 3,
                                    purchased_quantity = 0,
                                    purchase_limit_quantity = 10,
                                    consumption_resource_id = 4,
                                    min_price = 1000,
                                    max_price = 5500,
                                    growth_kind = 3
                                }
                            }
                        }
                    }
                }
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IList<ShopData> shops { get; set; } = new List<ShopData>();

            public class ShopData
            {
                public string id { get; set; } = "";
                public long start_at { get; set; } // unixtime
                public long end_at { get; set; } // unixtime
                public IDictionary<string, ShopItem> products { get; set; } = new Dictionary<string, ShopItem>();
            }

            public class ShopItem
            {
                public long id { get; set; }
                public long item_master_id { get; set; }
                public int item_category_id { get; set; }
                public int order { get; set; }
                public int purchased_quantity { get; set; }
                public int purchase_limit_quantity { get; set; }
                public int consumption_resource_id { get; set; }
                public int min_price { get; set; }
                public int max_price { get; set; }
                public int growth_kind { get; set; }
            }
        }
    }
}