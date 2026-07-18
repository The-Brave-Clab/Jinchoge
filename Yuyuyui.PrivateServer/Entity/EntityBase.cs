using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Titanium.Web.Proxy.EventArguments;
using Yuyuyui.GK;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer
{
    public abstract class EntityBase
    {
        public const string BASE_API_PATH = "/api/v1";
        public const string MIMETYPE_GK_JSON = "application/x-gk-json";
        public const string MIMETYPE_JSON = "application/json";

        public readonly string[] AcceptedHttpMethods;
        public readonly string HttpMethod;
        public readonly Uri RequestUri;
        protected byte[] requestBody;
        protected readonly Dictionary<string, string> requestHeaders;
        protected readonly Dictionary<string, string> pathParameters;
        protected byte[] responseBody;
        protected Dictionary<string, string> responseHeaders;


        public byte[] RequestBody => requestBody;
        public Dictionary<string, string> RequestHeaders => requestHeaders;
        public Dictionary<string, string> PathParameters => pathParameters;
        public byte[] ResponseBody => responseBody;
        public Dictionary<string, string> ResponseHeaders => responseHeaders;

        public bool HeaderContainsKey(string headerKey)
        {
            return requestHeaders.Any(header =>
                string.Equals(header.Key, headerKey, StringComparison.CurrentCultureIgnoreCase));
        }

        public string GetRequestHeaderValue(string headerKey)
        {
            foreach (var header in requestHeaders.Where(header =>
                         string.Equals(header.Key, headerKey, StringComparison.CurrentCultureIgnoreCase)))
            {
                return header.Value;
            }

            return "";
        }

        public string GetPathParameter(string key)
        {
            return pathParameters[key];
        }

        protected bool HasRequestBody()
        {
            return requestBody.Length != 0;
        }

        protected static string StripApiPrefix(string apiPath)
        {
            return apiPath.StartsWith(BASE_API_PATH) ? apiPath.Substring(BASE_API_PATH.Length) : apiPath;
        }

        private static Dictionary<string, string>? ExtractPathParameters(string apiPathWithParameters,
            string apiPathReal)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            var orig = apiPathWithParameters.Split(new [] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            var real = apiPathReal.Split(new [] {'/'}, StringSplitOptions.RemoveEmptyEntries);

            if (orig.Length != real.Length) return null;

            for (int i = 0; i < orig.Length; i++)
            {
                if (orig[i].StartsWith("{") && orig[i].EndsWith("}"))
                    result.Add(orig[i].Trim('{', '}'), real[i]);
                else if (orig[i] != real[i]) return null;
            }

            return result;
        }

        private static bool ApiPathMatch(string apiPathWithParameters, string apiPathReal)
        {
            return ExtractPathParameters(apiPathWithParameters, apiPathReal) != null;
        }

        public static async Task<EntityBase> FromRequestEvent(SessionEventArgs e)
        {
            string apiPath = StripApiPrefix(e.HttpClient.Request.RequestUri.AbsolutePath);

            Utils.LogTrace($"{e.HttpClient.Request.Method} {apiPath}");

            var headersAndBody = await ProxyUtils.GetRequestHeadersAndBody(e);

            foreach (var config in configs)
            {
                if (ApiPathMatch(config.Value.apiPath, apiPath) &&
                    config.Value.httpMethods.Contains(e.HttpClient.Request.Method))
                {
                    try
                    {
                        return (EntityBase) TypeDescriptor.CreateInstance(
                            provider: null,
                            objectType: config.Key,
                            argTypes: new[]
                            {
                                typeof(Uri),
                                typeof(string),
                                typeof(Dictionary<string, string>),
                                typeof(byte[]),
                                typeof(RouteConfig)
                            },
                            args: new object[]
                            {
                                e.HttpClient.Request.RequestUri,
                                e.HttpClient.Request.Method,
                                headersAndBody.Item1,
                                headersAndBody.Item2,
                                config.Value
                            })!;
                    }
                    catch (Exception exception)
                    {
                        Utils.LogError(exception.Message);
                        throw;
                    }
                }
            }

            string notImplementedMessage = $"{Resources.LOG_PS_API_NOT_IMPLEMENTED}{e.HttpClient.Request.Method} {apiPath}";
            Utils.LogWarning(notImplementedMessage);
            return new RequestErrorEntity(
                "A1321",
                notImplementedMessage,
                e.HttpClient.Request.RequestUri,
                e.HttpClient.Request.Method,
                new RouteConfig(apiPath, e.HttpClient.Request.Method),
                headersAndBody.Item1,
                headersAndBody.Item2,
                notImplementedMessage);
        }

        protected abstract Task ProcessRequest();

        public async Task Process()
        {
            if (requestBody.Length > 0)
            {
                if (GetRequestHeaderValue("Content-Type").ToLower() == MIMETYPE_GK_JSON)
                {
                    bool hasSessionCookie = this.GetSessionFromCookie(out var session);
                    if (!hasSessionCookie)
                    {
                        requestBody =
                            Config.Get().Security.UseOnlineDecryption
                                ? LibGK<LibGKLambda>.Execute(CryptType.API, CryptDirection.Decrypt, requestBody)
                                : LibGK<GoalKeeper>.Execute(CryptType.API, CryptDirection.Decrypt, requestBody);
                    }
                    else
                    {
                        requestBody =
                            Config.Get().Security.UseOnlineDecryption
                                ? LibGK<LibGKLambda>.Execute(CryptType.API, CryptDirection.Decrypt, requestBody,
                                    session.sessionKey, sessionKey: true)
                                : LibGK<GoalKeeper>.Execute(CryptType.API, CryptDirection.Decrypt, requestBody,
                                    session.sessionKey, sessionKey: true);
                    }
                }
            }

            try
            {
                await ProcessRequest();
            }
            catch (APIErrorException)
            {
                throw;
            }
            catch (Exception e)
            {
                Utils.LogError(e.Message);
                throw;
            }
        }

        protected static T? Deserialize<T>(byte[] data) where T : class
        {
            using var stream = new MemoryStream(data);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return JsonSerializer.Create().Deserialize(reader, typeof(T)) as T;
        }

        protected static byte[] Serialize<T>(T obj) where T : class
        {
            string str = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(str);
        }

        protected PlayerProfile GetPlayerFromCookies()
        {
            bool isSession = this.GetSessionFromCookie(out var playerSession);
            if (!isSession)
            {
                throw new APIErrorException("U0401", "Unauthorized Error");
            }

            return playerSession.player;
        }

        protected void SetBasicResponseHeaders(string sessionId = "", bool isGk = false)
        {
            responseHeaders.Add("Content-Type", isGk ? MIMETYPE_GK_JSON : MIMETYPE_JSON);
            responseHeaders.Add("Content-Length", $"{responseBody.Length}");
            if (!string.IsNullOrEmpty(sessionId))
                responseHeaders.Add("Set-Cookie", $"_session_id={sessionId}");
        }

        public EntityBase(Uri requestUri, string httpMethod, Dictionary<string, string> requestHeaders,
            byte[] requestBody, RouteConfig config)
        {
            AcceptedHttpMethods = config.httpMethods;
            HttpMethod = httpMethod;
            RequestUri = requestUri;
            this.requestHeaders = requestHeaders;
            this.requestBody = requestBody;
            responseBody = Array.Empty<byte>();
            responseHeaders = new Dictionary<string, string>();

            pathParameters = ExtractPathParameters(config.apiPath, StripApiPrefix(requestUri.AbsolutePath))!;
        }

        public static readonly RouteConfigCollection configs = new()
        {
            {
                typeof(TutorialProgressEntity),
                new RouteConfig("/my/tutorial_progress", "GET", "PUT")
            },
            {
                typeof(ArticleEntity),
                new RouteConfig("/articles", "GET")
            },
            {
                typeof(RegistrationsEntity),
                new RouteConfig("/registrations", "POST")
            },
            {
                typeof(SessionsEntity),
                new RouteConfig("/sessions", "POST")
            },
            {
                typeof(BanEntity),
                new RouteConfig("/my/profile/ban", "POST")
            },
            {
                typeof(RegulationEntity),
                new RouteConfig("/my/regulation_version", "GET", "PUT")
            },
            {
                typeof(HeaderEntity),
                new RouteConfig("/my/header", "GET")
            },
            {
                typeof(BadgeEntity),
                new RouteConfig("/my/badge", "GET", "PUT")
            },
            {
                typeof(BannerEntity),
                new RouteConfig("/banners", "GET")
            },
            {
                typeof(AccessoryListEntity),
                new RouteConfig("/my/accessories", "GET")
            },
            {
            	typeof(AccessoryEnhancementResultEntity),
            	new RouteConfig("/my/accessories/{accessory_id}", "PUT")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/my/inherited_password", "GET", "POST")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/inherited_executions", "POST")
            },
            {
                typeof(CooperationEntity),
                new RouteConfig("/portalsite/cooperations", "GET", "POST")
            },
            {
                typeof(CooperationEntity),
                new RouteConfig("/portalsite/cooperations/confirm", "GET", "POST")
            },
            {
                typeof(CooperationEntity),
                new RouteConfig("/portalsite/cooperations/execute", "POST")
            },
            {
                typeof(AchievementsEntity),
                new RouteConfig("/achievements", "GET", "POST", "PUT")
            },
            {
                typeof(AchievementsEntity),
                new RouteConfig("/my/achievements", "GET", "POST", "PUT")
            },
            {
                typeof(AchievementsEntity),
                new RouteConfig("/my/achievements/{achievement_id}", "GET", "POST", "PUT")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/my/brave_system/components", "GET")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/my/brave_system/components/{component_id}", "PUT")
            },
            {
                typeof(EpisodeEntity),
                new RouteConfig("/my/chapters/{chapter_id}/episodes", "GET")
            },
            {
                typeof(ChapterEntity),
                new RouteConfig("/my/chapters", "GET")
            },
            {
                typeof(StageEntity),
                new RouteConfig("/my/chapters/{chapter_id}/episodes/{episode_id}/stages", "GET")
            },
            {
            	typeof(GuestEntity),
            	new RouteConfig("/my/supporters", "GET")
            },
            {
            	typeof(CheckBattleTokensEntity),
            	new RouteConfig("/check_battle_tokens", "GET")
            },
            {
                typeof(EventChapterEntity),
                new RouteConfig("/special/chapters", "GET")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/special/chapters/{chapter_id}/episodes", "GET")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/special/chapters/{chapter_id}/episodes/{episode_id}/stages", "GET")
            },
            {
                typeof(BingoSheetsEntity),
                new RouteConfig("/my/bingo_sheets", "GET")
            },
            {
                typeof(BingoSheetsExchangeEntity),
                new RouteConfig("/my/bingo_sheets/{bingo_sheet_id}/exchanges", "POST")
            },
            {
                typeof(CardsEntity),
                new RouteConfig("/my/cards", "GET")
            },
            {
            	typeof(EnhancementResultTransactionCreateEntity),
            	new RouteConfig("/my/cards/{card_id}/enhancement/transactions", "POST")
            },
            {
            	typeof(EnhancementResultTransactionUpdateEntity),
            	new RouteConfig("/my/cards/{card_id}/enhancement/transactions/{transaction_id}", "PUT")
            },
            {
            	typeof(EvolutionCardResultEntity),
            	new RouteConfig("/my/cards/{card_id}/evolution", "PUT")
            },
            {
                typeof(ShopEntity),
                new RouteConfig("/shops", "GET")
            },
            {
                typeof(BirthdateRegistrationEntity),
                new RouteConfig("/my/birthdate_registration", "POST", "PUT")
            },
            {
                typeof(IABItemStatsEntity),
                new RouteConfig("/my/billing_point", "GET")
            },
            {
                typeof(BillingPointTransactionCreateEntity),
                new RouteConfig("/billing_point_shop/stamina_recovery/transactions", "POST")
            },
            {
                typeof(BillingPointTransactionUpdateEntity),
                new RouteConfig("/billing_point_shop/stamina_recovery/transactions/{transaction_id}", "PUT")
            },
            {
                typeof(BillingPointTransactionCreateEntity),
                new RouteConfig("/billing_point_shop/enhancement_item_capacity/transactions", "POST")
            },
            {
                typeof(BillingPointTransactionUpdateEntity),
                new RouteConfig("/billing_point_shop/enhancement_item_capacity/transactions/{transaction_id}", "PUT")
            },
            {
                typeof(ShopProductTransactionCreateEntity),
                new RouteConfig("/shops/{shop_id}/products/{product_id}/transactions", "POST")
            },
            {
                typeof(ShopProductTransactionUpdateEntity),
                new RouteConfig("/shops/{shop_id}/products/{product_id}/transactions/{transaction_id}", "PUT")
            },
            {
                typeof(UpdateUserPaymentEntity),
                new RouteConfig("/purchase/receipts", "POST")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/portalsite/dmm/payments", "POST")
            },
            {
                typeof(BillingItemListEntity),
                new RouteConfig("/platform_products", "GET")
            },
            {
                typeof(BillingPointShopEntity),
                new RouteConfig("/billing_point_shop", "GET")
            },
            {
            	typeof(ExchangeItemListEntity),
            	new RouteConfig("/exchange_booths", "GET")
            },
            {
                typeof(TradeBoothsEntity),
                new RouteConfig("/trade_booths", "GET")
            },
            {
                typeof(EventItemBoothsEntity),
                new RouteConfig("/event_item_booths", "GET")
            },
            {
                typeof(ExchangeItemCreateEntity),
                new RouteConfig("/exchange_booths/{exchange_list_id}/exchange_item/{exchange_item}/current", "GET")
            },
            {
                typeof(ExchangeItemUpdateEntity),
                new RouteConfig("/exchange_booths/{exchange_list_id}/exchange_item/{exchange_item}/exchange", "POST")
            },
            {
                typeof(EventBonusCardsEntity),
                new RouteConfig("/cards/event_bonus_cards", "GET")
            },
            {
                typeof(EventBonusCharacterCardsEntity),
                new RouteConfig("/cards/event_bonus_character_cards", "GET")
            },
            {
                typeof(GachaEntity),
                new RouteConfig("/gachas", "GET")
            },
            {
                typeof(SelectGachaEntity),
                new RouteConfig("/select_gachas/select_gacha_cards", "GET")
            },
            {
                typeof(UpdateUserSelectEntity),
                new RouteConfig("/select_gachas/update_user_select", "POST")
            },
            {
                typeof(GachaTicketEntity),
                new RouteConfig("/my/gacha_tickets", "GET")
            },
            {
                typeof(GachaTransactionCreateEntity),
                new RouteConfig("/gachas/{gacha_id}/lineups/{lineup_id}/transactions", "POST")
            },
            {
                typeof(GachaTransactionUpdateEntity),
                new RouteConfig("/gachas/{gacha_id}/lineups/{lineup_id}/transactions/{transaction_id}", "PUT")
            },
            {
                typeof(PresentsEntity),
                new RouteConfig("/my/gifts", "GET")
            },
            {
                typeof(PresentsHistoryEntity),
                new RouteConfig("/my/gifts/received", "GET")
            },
            {
                typeof(GiftAcceptEntity),
                new RouteConfig("/my/gifts/bulk", "POST", "PUT")
            },
            {
                typeof(GiftAcceptEntity),
                new RouteConfig("/my/gifts/{gift_id}", "PUT")
            },
            {
            	typeof(FriendEntity),
            	new RouteConfig("/my/fellowships", "GET")
            },
            {
            	typeof(DeleteFriendEntity),
            	new RouteConfig("/my/fellowships/{user_id}/removal", "POST")
            },
            {
            	typeof(FellowRequestEntity),
            	new RouteConfig("/my/fellow_requests", "GET")
            },
            {
            	typeof(UpdateFellowRequestEntity),
            	new RouteConfig("/my/fellow_requests/{request_id}", "PUT")
            },
            {
            	typeof(SendFellowRequestEntity),
            	new RouteConfig("/users/{user_id}/fellow_requests", "POST")
            },
            {
            	typeof(ClubWorkingSlotEntity),
            	new RouteConfig("/my/club_working/slots", "GET")
            },
            {
                typeof(ClubWorkingOrderEntity),
                new RouteConfig("/my/club_working/orders", "GET")
            },
            {
                typeof(ClubWorkingStartEntity),
                new RouteConfig("/my/club_working/workings/start", "POST")
            },
            {
                typeof(ClubWorkingRewardEntity),
                new RouteConfig("/my/club_working/workings/{club_working_id}/result", "PUT")
            },
            {
                typeof(ClubWorkingForceCompleteTransactionCreateEntity),
                new RouteConfig("/my/club_working/workings/{club_working_id}/transactions", "POST")
            },
            {
                typeof(ClubWorkingForceCompleteTransactionUpdateEntity),
                new RouteConfig("/my/club_working/workings/{club_working_id}/transactions/{transaction_id}", "PUT")
            },
            {
                typeof(UserInfoEntity),
                new RouteConfig("/users/{user_id}", "GET")
            },
            {
                typeof(ProfileEntity),
                new RouteConfig("/my/profile", "PUT")
            },
            {
                typeof(LoginBonusEntity),
                new RouteConfig("/my/checkin", "POST")
            },
            {
                typeof(MissionListEntity),
                new RouteConfig("/missions", "GET")
            },
            {
                typeof(MissionListEntity),
                new RouteConfig("/my/missions", "GET")
            },
            {
                typeof(DailyMissionListEntity),
                new RouteConfig("/daily_missions", "GET")
            },
            {
                typeof(DailyMissionListEntity),
                new RouteConfig("/my/daily_missions", "GET")
            },
            {
                typeof(MissionListEntity),
                new RouteConfig("/mission_adventures", "GET")
            },
            {
                typeof(MissionListEntity),
                new RouteConfig("/my/mission_adventures", "GET")
            },
            {
                typeof(MissionUpdateEntity),
                new RouteConfig("/my/missions/{mission_id}", "PUT")
            },
            {
                typeof(DailyMissionUpdateEntity),
                new RouteConfig("/my/daily_missions/{mission_id}", "PUT")
            },
            {
                typeof(MissionADVUpdateEntity),
                new RouteConfig("/my/mission_adventures/{mission_adventure_id}", "PUT")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/level_design/quests/{quest_id}/start", "POST")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/level_design/quests/{quest_id}/finish", "POST", "PUT")
            },
            {
                typeof(EnhancementItemsEntity),
                new RouteConfig("/my/enhancement_items", "GET")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/my/enhancement_items/{enhancement_item_id}/disposal", "POST", "PUT")
            },
            {
                typeof(StaminaItemsEntity),
                new RouteConfig("/my/stamina_items", "GET")
            },
            {
                typeof(StaminaItemTransactionCreateEntity),
                new RouteConfig("/my/stamina_items/{stamina_item_id}/transactions", "POST")
            },
            {
                typeof(StaminaItemTransactionUpdateEntity),
                new RouteConfig("/my/stamina_items/{stamina_item_id}/transactions/{transaction_id}", "PUT")
            },
            {
                typeof(EvolutionItemsEntity),
                new RouteConfig("/my/evolution_items", "GET")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/my/enhancement_items/{enhancement_item_id}/disposal/transactions", "POST")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/my/enhancement_items/{enhancement_item_id}/disposal/transactions/{transaction_id}", "PUT")
            },
            {
                typeof(TitleItemsEntity),
                new RouteConfig("/my/title_items", "GET", "POST")
            },
            {
                typeof(TitleItemsReadEntity),
                new RouteConfig("/my/title_items/read", "POST")
            },
            {
                typeof(TitleItemsCheckEntity),
                new RouteConfig("/my/title_items/check", "POST")
            },
            {
            	typeof(AlbumListEntity),
            	new RouteConfig("/my/adventure_books", "GET", "POST")
            },
            {
            	typeof(AlbumReadEntity),
            	new RouteConfig("/my/adventure_books/{adventure_books_id}", "PUT")
            },
            {
                typeof(DeckEntity),
                new RouteConfig("/my/decks", "GET")
            },
            {
            	typeof(DeckUpdateEntity),
            	new RouteConfig("/my/decks/{deck_id}", "PUT")
            },
            {
            	typeof(QuestTransactionCreateEntity),
            	new RouteConfig("/stages/{stage_id}/transactions", "POST")
            },
            {
            	typeof(QuestTransactionUpdateEntity),
            	new RouteConfig("/stages/{stage_id}/transactions/{transaction_id}", "PUT")
            },
            {
            	typeof(QuestTransactionResultEntity),
            	new RouteConfig("/stages/{stage_id}/transactions/{transaction_id}/result", "PUT")
            },
            {
            	typeof(QuestTransactionRetireEntity),
            	new RouteConfig("/stages/{stage_id}/transactions/{transaction_id}/retire", "PUT")
            },
            {
            	typeof(QuestTransactionDefeatEntity),
            	new RouteConfig("/stages/{stage_id}/transactions/{transaction_id}/defeat", "PUT")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/special/stages/{stage_id}/transactions", "POST")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/special/stages/{stage_id}/transactions/{transaction_id}", "PUT")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/special/stages/{stage_id}/transactions/{transaction_id}/result", "PUT")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/special/stages/{stage_id}/transactions/{transaction_id}/retire", "PUT")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/special/stages/{stage_id}/transactions/{transaction_id}/defeat", "PUT")
            },
            {
                typeof(BillingPointTransactionCreateEntity),
                new RouteConfig("/billing_point_shop/weekday_stamina_recovery/transactions", "POST")
            },
            {
                typeof(BillingPointTransactionUpdateEntity),
                new RouteConfig("/billing_point_shop/weekday_stamina_recovery/transactions/{transaction_id}", "PUT")
            },
            {
                typeof(CharacterFamiliarityEntity),
                new RouteConfig("/my/character_familiarities", "GET")
            },
            {
                typeof(GameResourceVersionEntity),
                new RouteConfig("/resource_versions/{type}", "GET")
            },
            {
                typeof(ScenarioResourceVersionEntity),
                new RouteConfig("/resource_versions/scenario/{scenario_id}", "GET")
            },
            //{
            //	typeof(MasterDataListEntity),
            //	new Config(string.Empty, string.Empty, 0)
            //},
            //{
            //	typeof(SoundListEntity),
            //	new Config(string.Empty, string.Empty, 0)
            //},
            {
                typeof(RequirementVersionEntity),
                new RouteConfig("/requirement_version", "GET")
            },
            {
                typeof(PushTokenEntity),
                new RouteConfig("/my/push_token", "POST")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/my/push_token/removal", "POST", "DELETE")
            },
            {
                typeof(NoopEntity),
                new RouteConfig("/campaigns/{campaign_id}/entry", "POST")
            },
            //{
            //	typeof(ResponseErrorEntity),
            //	new Config("http://toybox.kaeru-the-frog.xyz/404.html", string.Empty, 0)
            //},
            {
            	typeof(BattleContinueConfirmEntity),
            	new RouteConfig("/battle/transaction/validate", "POST")
            },
            {
            	typeof(BattleContinueEntity),
            	new RouteConfig("/battle/continue/transactions", "POST")
            },
            {
            	typeof(BattleContinueInfoEntity),
            	new RouteConfig("/battle/continue/transactions/{transaction_id}", "GET", "PUT")
            },

            // The following entities don't have mono code!
            {
                typeof(UpdateClickCountsEntity),
                new RouteConfig("/click_counts/update_click_counts", "POST")
            },
            {
                typeof(EventItemsEntity),
                new RouteConfig("/my/event_items", "GET")
            },
            {
                typeof(PopupEntity),
                new RouteConfig("/popups", "GET")
            },
            {
                typeof(BuffsEntity),
                new RouteConfig("/my/buffs", "GET")
            },
            {
                typeof(AutoClearTicketsEntity),
                new RouteConfig("/my/auto_clear_tickets", "GET")
            },
            {
                typeof(CharacterTitleItemsEntity),
                new RouteConfig("/my/character_title_items", "POST")
            }
        };
    }

    public struct RouteConfig
    {
        public RouteConfig(string apiPath, params string[] httpMethods)
        {
            this.apiPath = apiPath;
            this.httpMethods = httpMethods;
        }

        public readonly string apiPath;
        public readonly string[] httpMethods;
    }

    public class RouteConfigCollection : List<KeyValuePair<Type, RouteConfig>>
    {
        public void Add(Type entityType, RouteConfig routeConfig)
        {
            Add(new KeyValuePair<Type, RouteConfig>(entityType, routeConfig));
        }
    }
}
