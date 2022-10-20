using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class GachaEntity : BaseEntity<GachaEntity>
    {
        public GachaEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            RouteConfig config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();

            using var gachasDb = new GachasContext();

            Response responseObj = new()
            {
                gachas = GetCurrentActiveGachas(gachasDb, player).Select(g => new GachaProductData
                {
                    id = g.Id,
                    name = g.Name,
                    kind = g.Kind,
                    description = g.Description,
                    banner_id = g.Kind switch { 0 => 10080, 1 => 1, _ => g.Id },
                    start_at = g.StartAt.ToUnixTime(),
                    end_at = g.EndAt.ToUnixTime(),
                    lineups = GetGachaLineups(gachasDb, g), // It seems that the client doesn't respect this
                    detail_url = "", // TODO
                    caution_url = "", // TODO
                    pickup_content = GetFirstPickupContent(g),
                    order = g.Kind switch { 0 => 100000, 1 => 200000, _ => (int)g.Id }, // TODO
                    skip_type = g.SkipType,
                    popup_se_name = g.PopupSeName ?? "",
                    special_get_count = g.SpecialGetCount,
                    user_get_count = null, // TODO
                    get_down_gacha_count = 0, // TODO
                    get_down_count = 2611, // TODO
                    count_down_gacha = g.CountDownGacha,
                    select_gacha = g.SelectGacha,
                    select_count = g.SelectCount,
                    special_select = g.SpecialSelect,
                    no_display_end_at = g.NoDisplayEndAt,
                }).ToList()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public static IEnumerable<Gacha> GetCurrentActiveGachas(GachasContext gachasDb, PlayerProfile player)
        {
            // var currentTime = DateTime.UtcNow;
            //
            // return gachasDb.Gachas.ToList()
            //     .Where(g => g.StartAt.ToDateTime() < currentTime && g.EndAt.ToDateTime() > currentTime)
            //     .Where(g => g.MaxUserLevel == null || player.data.level <= g.MaxUserLevel)
            //     .Where(g => g.StepupGroup == null || g.Id == g.StepupGroup); // TODO: process step up

            return new List<Gacha>
            {
                GetCommonGacha(gachasDb),
                GetFriendGacha(gachasDb)
            };
        }

        private static Gacha GetCommonGacha(GachasContext gachasDb)
        {
            var commonGacha = gachasDb.Gachas.ToList()
                .Where(g => g.Kind == 0) // common gacha
                .MaxBy(g => g.EndAt.ToDateTime());
            commonGacha!.Name = "Private Server 勇者ガチャ";
            commonGacha!.EndAt = "2028/12/31 18:59:59";
            commonGacha!.NoDisplayEndAt = "1";
            return commonGacha;
        }

        private static Gacha GetFriendGacha(GachasContext gachasDb)
        {
            var friendGacha = gachasDb.Gachas.ToList()
                .Where(g => g.Kind == 1) // friend gacha
                .MaxBy(g => g.EndAt.ToDateTime());
            friendGacha!.Name = "Private Server " + friendGacha.Name;
            return friendGacha;
        }

        private List<GachaProductData.Lineup> GetGachaLineups(GachasContext gachasDb, Gacha gacha)
        {
            var lineups = gachasDb.GachaLineups
                .Where(l => l.GachaId == gacha.Id)
                .Where(l => l.Sp == 1) // We only need those on Smart Phone
                .Select(l =>
                    new GachaProductData.Lineup
                    {
                        id = l.Id,
                        lot_count = l.LotCount,
                        consumption_resource_id = l.ConsumptionResourceId,
                        consumption_amount = l.ConsumptionAmount,
                        consumable = true, // TODO
                        has_right = true, // TODO
                        button_extra = l.ButtonExtra,
                        button_title = l.ButtonTitle,
                        played_count = null, // TODO
                        has_bonus = false, // TODO
                        bonus_description = null // TODO
                    }).ToList();
            return lineups;
        }

        private List<GachaProductData.PickupContent> GetGachaPickUps(Gacha gacha)
        {
            if (gacha.PickupType == null || gacha.PickupId == null) 
                return new List<GachaProductData.PickupContent>();

            var pickupTypes = gacha.PickupType.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var pickupIds = gacha.PickupId.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (pickupTypes.Length != pickupIds.Length) 
                return new List<GachaProductData.PickupContent>();
            
            var result = new List<GachaProductData.PickupContent>(pickupTypes.Length);

            for (int i = 0; i < pickupTypes.Length; ++i)
            {
                int itemCategoryId;
                // only saw cards and accessories for now
                if (pickupTypes[i] == "Accessory")
                    itemCategoryId = 0;
                else if (pickupTypes[i] == "Card")
                    itemCategoryId = 1;
                else
                    itemCategoryId = -1;

                long masterId = long.Parse(pickupIds[i]);
                
                result.Add(new()
                {
                    item_category_id = itemCategoryId,
                    master_id = masterId
                });
            }

            return result;
        }

        private GachaProductData.PickupContent? GetFirstPickupContent(Gacha gacha)
        {
            var contents = GetGachaPickUps(gacha);
            if (contents.Count > 0) return contents[0];
            return null;
        }

        public class Response
        {
            public IList<GachaProductData> gachas { get; set; } = new List<GachaProductData>();
        }
    }
}