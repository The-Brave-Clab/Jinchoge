﻿using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer
{
    public abstract class BaseEntity<T> : EntityBase 
        where T : BaseEntity<T>
    {
        public BaseEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            RouteConfig config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }
    }
}