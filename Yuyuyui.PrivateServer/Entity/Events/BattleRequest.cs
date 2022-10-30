namespace Yuyuyui.PrivateServer.Events;

public class BattleRequest
{
    public int quest_id { get; set; }
    public BattleTransactionRequest transaction { get; set; }
}