using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class UpdateUserSelectEntity : BaseEntity<UpdateUserSelectEntity>
    {
        public UpdateUserSelectEntity(
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
            
            Request request = Deserialize<Request>(requestBody)!;
            
            player.gachaSelections[request.gacha_id] = request.select_ids;
            player.Save();

            // It seems that the client doesn't read this response.
            responseBody = Encoding.UTF8.GetBytes("{}");
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Request
        {
            public long gacha_id { get; set; }
            public IList<int> select_ids { get; set; } = new List<int>();
        }

        // public class Response
        // {
        //     public IList<UserSelect> update_user_select { get; set; } = new List<UserSelect>();
        //     public class UserSelect
        //     {
        //         public long id { get; set; }
        //         public string select_ids { get; set; }
        //         public long user_id { get; set; }
        //         public long cacha_id { get; set; }
        //         public string created_at { get; set; }
        //         public string updated_at { get; set; }
        //     }
        // }
    }
}