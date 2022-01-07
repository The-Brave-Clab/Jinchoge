using System.Net;

namespace Yuyuyui.PrivateServer
{
    public class GachaEntity : BaseEntity<GachaEntity>
    {
        public GachaEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config,
            IPEndPoint localEndPoint)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config, localEndPoint)
        {
        }

        protected override Task ProcessRequest()
        {
            Utils.LogWarning("Stub API! Only returns the 2 default gachas");

            Response responseObj = new()
            {
                gachas = new List<GachaProductData>
                {
                    new()
                    {
                        id = 11020,
                        name = "Private Server 勇者ガチャ",
                        kind = 0,
                        description = "勇者ガチャからはR、SR、SSRのいずれかの勇者が登場！10連ガチャからはSR以上の勇者が一人確定で出現！",
                        banner_id = 10080,
                        start_at = 1640977200,
                        end_at = 1861901999,
                        lineups = new List<GachaProductData.Lineup>
                        {
                            new()
                            {
                                id = 110201,
                                lot_count = 1,
                                consumption_resource_id = 1,
                                consumption_amount = 250,
                                consumable = false,
                                has_right = true,
                                button_extra = null,
                                button_title = null,
                                played_count = null,
                                has_bonus = false,
                                bonus_description = null
                            },
                            new()
                            {
                                id = 110202,
                                lot_count = 10,
                                consumption_resource_id = 1,
                                consumption_amount = 2300,
                                consumable = false,
                                has_right = true,
                                button_extra = null,
                                button_title = null,
                                played_count = null,
                                has_bonus = false,
                                bonus_description = null
                            },
                            new()
                            {
                                id = 110203,
                                lot_count = 1,
                                consumption_resource_id = 2,
                                consumption_amount = 60,
                                consumable = false,
                                has_right = true,
                                button_extra = null,
                                button_title = null,
                                played_count = null,
                                has_bonus = false,
                                bonus_description = null
                            },
                            new()
                            {
                                id = 110204,
                                lot_count = 1,
                                consumption_resource_id = 2001,
                                consumption_amount = 1,
                                consumable = false,
                                has_right = false,
                                button_extra = null,
                                button_title = null,
                                played_count = null,
                                has_bonus = false,
                                bonus_description = null
                            }
                        },
                        detail_url = "https://article.yuyuyui.jp/article/gachas/11020",
                        caution_url = "https://article.yuyuyui.jp/article/gachas/11020/caution",
                        pickup_content = new()
                        {
                            item_category_id = 1,
                            master_id = 300010
                        },
                        order = 100000,
                        skip_type = 0,
                        popup_se_name = "",
                        special_get_count = null,
                        user_get_count = null,
                        get_down_gacha_count = 0,
                        get_down_count = 2611,
                        count_down_gacha = null,
                        select_gacha = null,
                        select_count = null,
                        special_select = null,
                        no_display_end_at = null
                    },
                    new()
                    {
                        id = 20080,
                        name = "Private Server フレンドガチャ",
                        kind = 1,
                        description = "フレンドptを使って「結城友奈は勇者である」で登場した精霊や「結城友奈は勇者である 花結いのきらめき」オリジナル精霊をGET！！\n勇者が獲得できることも！？",
                        banner_id = 1,
                        start_at = 1614538800,
                        end_at = 1861901999,
                        lineups = new List<GachaProductData.Lineup>
                        {
                            new()
                            {
                                id = 20080,
                                lot_count = 1,
                                consumption_resource_id = 3,
                                consumption_amount = 200,
                                consumable = false,
                                has_right = true,
                                button_extra = null,
                                button_title = null,
                                played_count = null,
                                has_bonus = false,
                                bonus_description = null
                            },
                            new()
                            {
                                id = 20081,
                                lot_count = 10,
                                consumption_resource_id = 3,
                                consumption_amount = 1500,
                                consumable = false,
                                has_right = true,
                                button_extra = null,
                                button_title = null,
                                played_count = null,
                                has_bonus = false,
                                bonus_description = null
                            }
                        },
                        detail_url = "https://article.yuyuyui.jp/article/gachas/20080",
                        caution_url = "https://article.yuyuyui.jp/article/gachas/20080/caution",
                        pickup_content = new()
                        {
                            item_category_id = 0,
                            master_id = 300000
                        },
                        order = 200000,
                        skip_type = 0,
                        popup_se_name = "",
                        special_get_count = null,
                        user_get_count = null,
                        get_down_gacha_count = 0,
                        get_down_count = 1700,
                        count_down_gacha = null,
                        select_gacha = null,
                        select_count = null,
                        special_select = null,
                        no_display_end_at = "1"
                    }
                }
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IList<GachaProductData> gachas { get; set; } = new List<GachaProductData>();
        }
    }
}