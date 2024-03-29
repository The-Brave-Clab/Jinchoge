﻿using System;
using System.Collections.Generic;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer;

public class EventChapterEntity : ChapterEntity
{
    public EventChapterEntity(
        Uri requestUri,
        string httpMethod,
        Dictionary<string, string> requestHeaders,
        byte[] requestBody,
        RouteConfig config)
        : base(requestUri, httpMethod, requestHeaders, requestBody, config)
    {
    }

    protected override Response GetChapters(QuestsContext questsDb)
    {
        // Utils.LogWarning("Stub API! Returns nothing for now.");
        return new()
        {
            chapters = new Dictionary<long, ChapterEntity.Response.Chapter>()
        };
    }
}