﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class PopupEntity : BaseEntity<PopupEntity>
    {
        public PopupEntity(
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
            // Utils.LogWarning("Stub API! Returns nothing for now.");

            Response responseObj = new()
            {
                popups = new List<Response.Popup>()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IList<Popup> popups { get; set; } = new List<Popup>();

            public class Popup
            {
                public int id { get; set; }
                public long image_id { get; set; }
                public int display_type { get; set; } // 1: with Don't show anymore, 2: without
            }
        }
    }
}