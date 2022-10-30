using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer.Events;

public class EventEpisodeEntity : BaseEntity<EventEpisodeEntity>
{
    public EventEpisodeEntity(
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
        var player = GetPlayerFromCookies();

        Utils.Log("Path parameters:");
        foreach (var pathParameter in pathParameters)
        {
            Utils.Log($"\t{pathParameter.Key} = {pathParameter.Value}");
        }
        
        long chapterId = long.Parse(GetPathParameter("specialChapterId"));

        Utils.LogWarning("Finished status not filled!");
        new QuestsContext();
        using var eventStoriesDb = new EventStoriesContext();
        Response responseObj = new()
        {
            episodes = eventStoriesDb.SpecialEpisodes
                .Where(e => e.SpecialChapterId == chapterId)
                .Select(e => Response.SpecialEpisode.GetFromDatabase(e, player))
                .ToDictionary(e => e.id, e => e)
        };
        
        responseBody = Serialize(responseObj);
        SetBasicResponseHeaders();

        return Task.CompletedTask;
    }
    
    public class Response
    {
        public IDictionary<long, SpecialEpisode> episodes { get; set; } = new Dictionary<long, SpecialEpisode>();

        public class SpecialEpisode
        {
            public long id { get; set; } // When dealing with transaction, this should be the id of the player progress
            public long master_id { get; set; } // Don't know the difference
            public bool finish { get; set; }
            public string detail_url { get; set; } = "";

            public static SpecialEpisode GetFromDatabase(Yuyuyui.PrivateServer.DataModel.SpecialEpisode dbEpisode,
                PlayerProfile player)
            {
                return new()
                {
                    id = dbEpisode.Id,
                    master_id = dbEpisode.Id,
                    finish = false,
                    detail_url = ""
                };
            }
        }
    }
}