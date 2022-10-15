﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class AutoClearTicketsEntity : BaseEntity<AutoClearTicketsEntity>
    {
        public AutoClearTicketsEntity(
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

            Response responseObj = new()
            {
                tickets = player.items.autoClearTickets
                    .Select(p => p.Value)
                    .Select(Item.Load)
                    .Where(t => t.quantity > 0)
                    .ToList()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IList<Item> tickets { get; set; } = new List<Item>();
        }
    }
}