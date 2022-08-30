namespace Yuyuyui.PrivateServer;

public class GachaProductData
{
    public long id { get; set; }
    public string name { get; set; } = "";
    public string description { get; set; } = "";
    public long start_at { get; set; } // unixtime
    public long end_at { get; set; } // unixtime
    public int order { get; set; }
    public int? skip_type { get; set; } // ?
    public string popup_se_name { get; set; } = ""; // ?
    public int? special_get_count { get; set; } = null; // ?
    public int? user_get_count { get; set; } = null; // ?
    public int get_down_gacha_count { get; set; } // ?
    public int get_down_count { get; set; } // ?
    public int? count_down_gacha { get; set; } = null; // Type not confirmed
    public string? select_gacha { get; set; } = null; // ? 
    public int? select_count { get; set; } // ?
    public string? special_select { get; set; } // ?
    public string? no_display_end_at { get; set; } // ?
    public long banner_id { get; set; }
    public int kind { get; set; } // 0: Billing, 1: Friend, 2: Limited, 3: Ticket
    public string detail_url { get; set; } = "";
    public string caution_url { get; set; } = "";
    public IList<Lineup> lineups { get; set; } = new List<Lineup>();
    public PickupContent? pickup_content { get; set; } = new();

    public class Lineup
    {
        // public bool IsFreePlay => consumption_amount == 1 && consumption_resource_id == ConsumptionResourceId.ConsumptionResource;
        //
        // public int DisplayConsumptionAmount
        // {
        // 	get
        // 	{
        // 		int num = consumption_amount;
        // 		if (IsFreePlay)
        // 		{
        // 			num = ((lot_count != 1) ? num : 250);
        // 		}
        // 		return num;
        // 	}
        // }
        //
        // public bool GachaButtonEnable
        // {
        // 	get
        // 	{
        // 		if (consumption_resource_id == ConsumptionResourceId.BillingPoint)
        // 		{
        // 			return true;
        // 		}
        // 		if (consumption_resource_id == ConsumptionResourceId.DailyBillingPoint || consumption_resource_id == ConsumptionResourceId.ConsumptionResource)
        // 		{
        // 			return consumable || has_right;
        // 		}
        // 		return consumable;
        // 	}
        // }

        public long id { get; set; }
        public bool consumable { get; set; }
        public bool has_right { get; set; }
        public int consumption_resource_id { get; set; }
        public int consumption_amount { get; set; }
        public int lot_count { get; set; }
        public string? button_title { get; set; } = null;
        public string? button_extra { get; set; } = null;
        public int? played_count { get; set; } = null;
        public bool has_bonus { get; set; }
        public string? bonus_description { get; set; } = null;

        // public enum ConsumptionResourceId
        // {
        // 	BillingPoint = 1,
        // 	DailyBillingPoint,
        // 	FriendPoint,
        // 	GameCoinPoint,
        // 	TicketSingle = 6,
        // 	TicketMulti,
        // 	BraveCoinPoint,
        // 	ExchangePoint = 21,
        // 	ConsumptionResource = 2001
        // }
    }

    public class PickupContent
    {
        public int item_category_id { get; set; }
        public long master_id { get; set; }
    }
}