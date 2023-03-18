using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer;

public class QuestTransactionDefeatEntity : QuestTransactionRetireEntity
{
    public QuestTransactionDefeatEntity(
        Uri requestUri,
        string httpMethod,
        Dictionary<string, string> requestHeaders,
        byte[] requestBody,
        RouteConfig config)
        : base(requestUri, httpMethod, requestHeaders, requestBody, config)
    {
    }
}