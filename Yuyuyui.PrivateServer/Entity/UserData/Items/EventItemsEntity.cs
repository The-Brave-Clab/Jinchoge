﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class EventItemsEntity : BaseEntity<EventItemsEntity>
    {
        public EventItemsEntity(
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
                event_items = player.items.eventItems
                    .Select(p => p.Value)
                    .Select(Item.Load)
                    .Where(ei => ei.quantity > 0) // don't show consumed items
                    .ToDictionary(ei => ei.id, ei => ei)
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IDictionary<long, Item> event_items { get; set; } = new Dictionary<long, Item>();
        }
    }
}