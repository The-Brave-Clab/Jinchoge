namespace Yuyuyui.PrivateServer;

public class EventChapterEntity : ChapterEntity
{
    public EventChapterEntity(
        Uri requestUri,
        string httpMethod,
        Dictionary<string, string> requestHeaders,
        byte[] requestBody,
        Config config)
        : base(requestUri, httpMethod, requestHeaders, requestBody, config)
    {
    }

    protected override Response GetChapters()
    {
        Utils.LogWarning("Stub API! Returns nothing for now.");
        return new()
        {
            chapters = new Dictionary<long, Chapter>()
        };
    }
}