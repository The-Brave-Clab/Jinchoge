namespace Yuyuyui.PrivateServer.DataModel.Data;

public class BattleDeck
{
    public LeaderSkillInfo friend_leader_skill { get; set; }

    public LeaderSkillInfo leader_skill { get; set; }

    public PassiveSkillInfo[] stage_leader_skills { get; set; } = Array.Empty<PassiveSkillInfo>();

    public BattleCardData[] cards { get; set; }
}