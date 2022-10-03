using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class BillingItemListEntity : BaseEntity<BillingItemListEntity>
    {
        public BillingItemListEntity(
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
            Response responseObj = new()
            {
                birthdate_registration = false,
                products = new List<Response.Product>()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public bool birthdate_registration { get; set; }
            public IList<Product> products { get; set; } = new List<Product>();

            public class Product
            {
                public string product_id { get; set; } = "";
                public int price { get; set; }
                public int point { get; set; }
                public string name { get; set; } = "";
                public Extra? extra { get; set; } = null;

                public class Extra
                {
                    public int item_category_id { get; set; }
                    public int item_id { get; set; }
                    public int quantity { get; set; }
                    public string description { get; set; } = "";
                    public int purchase_limit_quantity { get; set; }
                    public int purchased_quantity { get; set; }
                    public long end_at { get; set; } // unixtime
                }
            }
        }
    }
}