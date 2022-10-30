namespace Yuyuyui.PrivateServer.Events;

public class BattleTransactionRequest
{
    public long id { get; set; }
    public long using_deck_id { get; set; }
    public long supporting_deck_card_id { get; set; }
    public bool no_friend { get; set; }
}