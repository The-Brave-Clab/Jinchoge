using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    protected override Task<Response> GetChapters()
    {
        // Utils.LogWarning("Stub API! Returns nothing for now.");
        return Task.FromResult(new Response
        {
            chapters = new Dictionary<long, ChapterEntity.Response.Chapter>()
        });
    }
}