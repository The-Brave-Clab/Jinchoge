using Yuyuyui.PrivateServer.Booth;

namespace Yuyuyui.PrivateServer.Entity;

public class BoothEventResponse
{
    public List<BoothExchange> exchange { get; set; } = new List<BoothExchange>();
}