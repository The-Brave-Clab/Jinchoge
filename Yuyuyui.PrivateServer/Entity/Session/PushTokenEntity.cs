﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class PushTokenEntity : BaseEntity<PushTokenEntity>
    {
        public PushTokenEntity(
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
            // We don't care about request body at all. 
            // This API won't be implemented anyway.
            Response responseObj = new() { status = "ok" };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public string status { get; set; } = "ok";
        }
    }
}