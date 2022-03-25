﻿using System.Text;

namespace Yuyuyui.PrivateServer
{
    public class CharacterTitleItemsEntity : BaseEntity<CharacterTitleItemsEntity>
    {
        public CharacterTitleItemsEntity(
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
            Utils.LogError("Request is {}, response is {}, WTF IS THIS ONE");

            responseBody = Encoding.UTF8.GetBytes("{}");
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }
    }
}