using System;
using System.Collections.Generic;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer.Entity.Common.Stage;

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
        long fakeId = 1591;
        Chapter chapter = new Chapter
        {
            id = 1591,
            master_id = 1591,
            kind = 2,
            start_at = 0,
            end_at = 0,
            detail_url = "",
            stack_point = 0,
            locked = false, // TODO
            new_released = false,
            completed = false,
            available_user_level = 0 // TODO
        };

        Dictionary<long, Chapter> responseChapters = new Dictionary<long, Chapter>();
        responseChapters.Add(1591, chapter);
        
        return new()
        {
            chapters = responseChapters
        };
    }
}