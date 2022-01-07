using System.Net;

namespace Yuyuyui.PrivateServer
{

    public class EventChapterEntity : ChapterEntity
    {
        public EventChapterEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config,
            IPEndPoint localEndPoint)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config, localEndPoint)
        {
        }

        protected override Response GetChapters()
        {
            Utils.LogWarning("Stub API! Returns nothing for now.");
            return new()
            {
                chapters = new Dictionary<int, Chapter>()
            };
        }
    }
}