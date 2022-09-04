using Yuyuyui.PrivateServer.Responses.Booth;

namespace Yuyuyui.PrivateServer.Responses;

public class BoothEventResponse
{
    public List<BoothExchange> exchange { get; set; } = new List<BoothExchange>();
}