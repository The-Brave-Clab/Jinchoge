namespace Yuyuyui.PrivateServer.DataModel.Data;

public class BattleCardData
{
    public BattleCardBaseInfo base_info { get; set; }
    public SupporterCardData supporter { get; set; }
    public SupporterCardData supporter_2 { get; set; }
    public int order { get; set; }
    public ActiveSkillInfo active_skill { get; set; }
    public AccessoryInfo[] accessories { get; set; } = Array.Empty<AccessoryInfo>();
    public PassiveSkillInfo[] passive_skills { get; set; }
    public int hp { get; set; }
    public long id { get; set; }
    public bool leader { get; set; }
    public int friend_type { get; set; }
    // public AssistCardData assist { get; set; }
    public SupporterCardData assist { get; set; }
}