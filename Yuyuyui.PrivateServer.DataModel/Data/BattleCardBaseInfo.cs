namespace Yuyuyui.PrivateServer.DataModel.Data;

public class BattleCardBaseInfo
{
	public int element { get; set; }
	public long character_type { get; set; }
	public long character_voice_type { get; set; }
	public float radius { get; set; }
	public int attack { get; set; }
	public float attack_radius { get; set; }
	public float attack_pace { get; set; }
	public float move_speed { get; set; }
	public int defense { get; set; }
	public float avoid_rate { get; set; }
	public float hit_rate { get; set; }
	public int critical_point { get; set; }
	public int footing_point { get; set; }
	public bool different_line_attack { get; set; }
	public long master_id { get; set; }
	public int attack_type { get; set; }
}