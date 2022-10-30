namespace Yuyuyui.PrivateServer.Requests;

public class SpecialBattleRequest
{
    public long id { get; set; }
    public long stage_id { get; set; }
    public long using_deck_id { get; set; }
    public long supporting_deck_card_id { get; set; }
    public bool no_friend { get; set; }
}