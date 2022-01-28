﻿using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Exceptions;

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

            var orig = apiPathWithParameters.Split("/", StringSplitOptions.RemoveEmptyEntries);
            var real = apiPathReal.Split("/", StringSplitOptions.RemoveEmptyEntries);

            if (orig.Length != real.Length) return null;

            for (int i = 0; i < orig.Length; i++)
            {
                if (orig[i].StartsWith('{') && orig[i].EndsWith('}'))
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

            var headersAndBody = await Proxy.GetRequestHeadersAndBody(e);

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
                                typeof(Config)
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
                        Utils.LogError(exception);
                        throw;
                    }
                }
            }

            return new RequestErrorEntity(
                "S2000",
                $"\n\nAPI Not Implemented:\n\n{e.HttpClient.Request.Method} {apiPath}",
                e.HttpClient.Request.RequestUri,
                e.HttpClient.Request.Method,
                new Config(apiPath, e.HttpClient.Request.Method),
                headersAndBody.Item1,
                headersAndBody.Item2,
                $"API Not Implemented: {e.HttpClient.Request.Method} {apiPath}"
            ); // error type
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
                        requestBody = await LibgkLambda.InvokeLambda(
                            LibgkLambda.CryptType.API,
                            LibgkLambda.CryptDirection.Decrypt,
                            requestBody); //, currentKey, currentIV, currentSessionKey);
                    }
                    else
                    {
                        requestBody = await LibgkLambda.InvokeLambda(
                            LibgkLambda.CryptType.API,
                            LibgkLambda.CryptDirection.Decrypt,
                            requestBody,
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
                Utils.LogError(e.ToString());
                throw;
            }
        }

        protected static T? Deserialize<T>(byte[] data) where T : class
        {
            var stream = new MemoryStream(data);
            var reader = new StreamReader(stream, Encoding.UTF8);
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
                throw new Exception("Session not found!");
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
            byte[] requestBody, Config config)
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

        public static readonly Dictionary<Type, Config> configs = new()
        {
            {
                typeof(TutorialProgressEntity),
                new Config("/my/tutorial_progress", "GET", "PUT")
            },
            {
                typeof(ArticleEntity),
                new Config("/articles", "GET")
            },
            {
                typeof(RegistrationsEntity),
                new Config("/registrations", "POST")
            },
            {
                typeof(SessionsEntity),
                new Config("/sessions", "POST")
            },
            {
                typeof(RegulationEntity),
                new Config("/my/regulation_version", "GET", "PUT")
            },
            {
                typeof(HeaderEntity),
                new Config("/my/header", "GET")
            },
            {
                typeof(BadgeEntity),
                new Config("/my/badge", "GET", "PUT")
            },
            {
                typeof(BannerEntity),
                new Config("/banners", "GET")
            },
            {
                typeof(AccessoryListEntity),
                new Config("/my/accessories", "GET")
            },
            //{
            //	typeof(AccessoryEnhancementResultEntity),
            //	new Config("/my/accessories/{0}", "Json/Accessory/accessory_effect", 0)
            //},
            //{
            //	typeof(MenuUserTransferEntity),
            //	new Config("/my/inherited_password", "Json/Menu/transfer", 0)
            //},
            //{
            //	typeof(InheritedExcuteEntity),
            //	new Config("/inherited_executions", string.Empty, 0)
            //},
            //{
            //	typeof(CooperationIssueEntity),
            //	new Config("/portalsite/cooperations", string.Empty, 0)
            //},
            //{
            //	typeof(CooperationConfirmEntity),
            //	new Config("/portalsite/cooperations/confirm", string.Empty, 0)
            //},
            //{
            //	typeof(CooperationExecuteEntity),
            //	new Config("/portalsite/cooperations/execute", string.Empty, 0)
            //},
            //{
            //	typeof(BraveSystemListEntity),
            //	new Config("/my/brave_system/components", "Json/Strategy/brave_user", 0)
            //},
            //{
            //	typeof(BraveSystemEnhacementEntity),
            //	new Config("/my/brave_system/components/{0}", "Json/Strategy/brave_enhance", 0)
            //},
            {
                typeof(EpisodeEntity),
                new Config("/my/chapters/{chapter_id}/episodes", "GET")
            },
            {
                typeof(ChapterEntity),
                new Config("/my/chapters", "GET")
            },
            {
                typeof(StageEntity),
                new Config("/my/chapters/{chapter_id}/episodes/{episode_id}/stages", "GET")
            },
            //{
            //	typeof(GuestEntity),
            //	new Config("/my/supporters", "Json/Story/guest", 0)
            //},
            //{
            //	typeof(CheckBattleTokensEntity),
            //	new Config("/check_battle_tokens", string.Empty, 0)
            //},
            {
                typeof(EventChapterEntity),
                new Config("/special/chapters", "GET")
            },
            //{
            //	typeof(EventEpisodeEntity),
            //	new Config("/special/chapters/{0}/episodes", string.Empty, 0)
            //},
            //{
            //	typeof(EventStageEntity),
            //	new Config("/special/chapters/{0}/episodes/{1}/stages", string.Empty, 0)
            //},
            {
                typeof(CardsEntity),
                new Config("/my/cards", "GET")
            },
            {
            	typeof(EnhancementResultTransactionCreateEntity),
            	new Config("/my/cards/{card_id}/enhancement/transactions", "POST")
            },
            {
            	typeof(EnhancementResultTransactionUpdateEntity),
            	new Config("/my/cards/{card_id}/enhancement/transactions/{transaction_id}", "PUT")
            },
            {
            	typeof(EvolutionCardResultEntity),
            	new Config("/my/cards/{card_id}/evolution", "PUT")
            },
            {
                typeof(ShopEntity),
                new Config("/shops", "GET")
            },
            //{
            //	typeof(BirthdateRegistrationEntity),
            //	new Config("/my/birthdate_registration", "Json/Shop/age_authorize", 0)
            //},
            {
                typeof(IABItemStatsEntity),
                new Config("/my/billing_point", "GET")
            },
            //{
            //	typeof(StaminaRecoveryTransactionCreateEntity),
            //	new Config("/billing_point_shop/stamina_recovery/transactions", "Json/Shop/recover_stamina_create", 0)
            //},
            //{
            //	typeof(StaminaRecoveryTransactionUpdateEntity),
            //	new Config("/billing_point_shop/stamina_recovery/transactions/{0}", "Json/Shop/recover_stamina_update", 0)
            //},
            //{
            //	typeof(BoxCapacityTransactionCreateEntity),
            //	new Config("/billing_point_shop/enhancement_item_capacity/transactions", "Json/Shop/box_capacity_create", 0)
            //},
            //{
            //	typeof(BoxCapacityTransactionUpdateEntity),
            //	new Config("/billing_point_shop/enhancement_item_capacity/transactions/{0}", "Json/Shop/box_capacity_update", 0)
            //},
            //{
            //	typeof(ShopProductTransactionCreateEntity),
            //	new Config("/shops/{0}/products/{1}/transactions", "Json/Shop/shop_product_create", 0)
            //},
            //{
            //	typeof(ShopProductTransactionUpdateEntity),
            //	new Config("/shops/{0}/products/{1}/transactions/{2}", "Json/Shop/shop_product_update", 0)
            //},
            //{
            //	typeof(UpdateUserPaymentEntity),
            //	new Config("/purchase/receipts", "Json/Shop/update_user_payment", 0)
            //},
            //{
            //	typeof(DMMUserPaymentEntity),
            //	new Config("/portalsite/dmm/payments", string.Empty, 0)
            //},
            {
                typeof(BillingItemListEntity),
                new Config("/platform_products", "GET")
            },
            {
                typeof(BillingPointShopEntity),
                new Config("/billing_point_shop", "GET")
            },
            //{
            //	typeof(ExchangeItemListEntity),
            //	new Config("/exchange_booths", "Json/ExchangeBooth/exchange_item_list", 0)
            //},
            //{
            //	typeof(ExchangeItemCreateEntity),
            //	new Config("/exchange_booths/{0}/exchange_item/{1}/current", "Json/ExchangeBooth/exchange_item_transaction_create", 0)
            //},
            //{
            //	typeof(ExchangeItemUpdateEntity),
            //	new Config("/exchange_booths/{0}/exchange_item/{1}/exchange", "Json/ExchangeBooth/exchange_item_transaction_update", 0)
            //},
            {
                typeof(EventBonusCardsEntity),
                new Config("/cards/event_bonus_cards", "GET")
            },
            {
                typeof(EventBonusCharacterCardsEntity),
                new Config("/cards/event_bonus_character_cards", "GET")
            },
            {
                typeof(GachaEntity),
                new Config("/gachas", "GET")
            },
            //{
            //	typeof(GachaTicketEntity),
            //	new Config("/my/gacha_tickets", "Json/Gacha/ticket", 0)
            //},
            //{
            //	typeof(GachaTransactionCreateEntity),
            //	new Config("/gachas/{0}/lineups/{1}/transactions", "Json/Gacha/transaction_create", 0)
            //},
            //{
            //	typeof(GachaTransactionUpdateEntity),
            //	new Config("/gachas/{0}/lineups/{1}/transactions/{2}", "Json/Gacha/transaction_update", 0)
            //},
            {
                typeof(PresentsEntity),
                new Config("/my/gifts", "GET")
            },
            {
                typeof(PresentsHistoryEntity),
                new Config("/my/gifts/received", "GET")
            },
            //{
            //	typeof(UpdatePresentEntity),
            //	new Config("/my/gifts/{0}", "Json/Present/update_present", 0)
            //},
            //{
            //	typeof(PresentBulkEntity),
            //	new Config("/my/gifts/bulk", string.Empty, 0)
            //},
            {
            	typeof(FriendEntity),
            	new Config("/my/fellowships", "GET")
            },
            {
            	typeof(DeleteFriendEntity),
            	new Config("/my/fellowships/{user_id}/removal", "POST")
            },
            {
            	typeof(FellowRequestEntity),
            	new Config("/my/fellow_requests", "GET")
            },
            {
            	typeof(UpdateFellowRequestEntity),
            	new Config("/my/fellow_requests/{request_id}", "PUT")
            },
            {
            	typeof(SendFellowRequestEntity),
            	new Config("/users/{user_id}/fellow_requests", "POST")
            },
            {
            	typeof(ClubWorkingSlotEntity),
            	new Config("/my/club_working/slots", "GET")
            },
            {
                typeof(ClubWorkingOrderEntity),
                new Config("/my/club_working/orders", "GET")
            },
            //{
            //	typeof(ClubWorkingStartEntity),
            //	new Config("/my/club_working/workings/start", "Json/ClubWorking/Start", 0)
            //},
            //{
            //	typeof(ClubWorkingRewardEntity),
            //	new Config("/my/club_working/workings/{0}/result", "Json/ClubWorking/Reward", 0)
            //},
            //{
            //	typeof(ClubWorkingForceCompleteTransactionCreateEntity),
            //	new Config("/my/club_working/workings/{0}/transactions", "Json/ClubWorking/complete_transaction_create", 0)
            //},
            //{
            //	typeof(ClubWorkingForceCompleteTransactionUpdateEntity),
            //	new Config("/my/club_working/workings/{0}/transactions/{1}", "Json/ClubWorking/complete_transaction_update", 0)
            //},
            {
                typeof(UserInfoEntity),
                new Config("/users/{user_id}", "GET")
            },
            {
                typeof(ProfileEntity),
                new Config("/my/profile", "PUT")
            },
            {
                typeof(LoginBonusEntity),
                new Config("/my/checkin", "POST")
            },
            //{
            //	typeof(MissionListEntity),
            //	new Config("/my/missions", "Json/Mission/mission_list", 0)
            //},
            //{
            //	typeof(DailyMissionListEntity),
            //	new Config("/my/daily_missions", string.Empty, 0)
            //},
            //{
            //	typeof(MissionUpdateEntity),
            //	new Config("/my/missions/{0}", "Json/Mission/mission_update", 0)
            //},
            //{
            //	typeof(DailyMissionUpdateEntity),
            //	new Config("/my/daily_missions/{0}", "Json/Mission/mission_update", 0)
            //},
            //{
            //	typeof(MissionADVUpdateEntity),
            //	new Config("/my/mission_adventures/{0}", string.Empty, 0)
            //},
            //{
            //	typeof(BattlePrepareEntity),
            //	new Config("/level_design/quests/{0}/start", "Json/Battle/battle_prepare", 0)
            //},
            //{
            //	typeof(BattleResultEntity),
            //	new Config("/level_design/quests/{0}/finish", "Json/Battle/battle_result", 0)
            //},
            {
                typeof(EnhancementItemsEntity),
                new Config("/my/enhancement_items", "GET")
            },
            //{
            //	typeof(EnhancementItemDisposalEntity),
            //	new Config("/my/enhancement_items/{0}/disposal", string.Empty, 0)
            //},
            {
                typeof(StaminaItemsEntity),
                new Config("/my/stamina_items", "GET")
            },
            //{
            //	typeof(StaminaTransactionCreateEntity),
            //	new Config("/my/stamina_items/{0}/transactions", string.Empty, 0)
            //},
            //{
            //	typeof(StaminaTransactionUpdateEntity),
            //	new Config("/my/stamina_items/{0}/transactions/{1}", string.Empty, 0)
            //},
            {
                typeof(EvolutionItemsEntity),
                new Config("/my/evolution_items", "GET")
            },
            //{
            //	typeof(EnhancementItemDisposalTransactionCreateEntity),
            //	new Config("/my/enhancement_items/{0}/disposal/transactions", string.Empty, 0)
            //},
            //{
            //	typeof(EnhancementItemDisposalTransactionUpdateEntity),
            //	new Config("/my/enhancement_items/{0}/disposal/transactions/{1}", string.Empty, 0)
            //},
            {
            	typeof(AlbumListEntity),
            	new Config("/my/adventure_books", "GET", "POST")
            },
            {
            	typeof(AlbumReadEntity),
            	new Config("/my/adventure_books/{adventure_books_id}", "PUT")
            },
            {
                typeof(DeckEntity),
                new Config("/my/decks", "GET")
            },
            {
            	typeof(DeckUpdateEntity),
            	new Config("/my/decks/{deck_id}", "PUT")
            },
            {
            	typeof(QuestTransactionCreateEntity),
            	new Config("/stages/{stage_id}/transactions", "POST")
            },
            {
            	typeof(QuestTransactionUpdateEntity),
            	new Config("/stages/{stage_id}/transactions/{transaction_id}", "PUT")
            },
            {
            	typeof(QuestTransactionResultEntity),
            	new Config("/stages/{stage_id}/transactions/{transaction_id}/result", "PUT")
            },
            //{
            //	typeof(QuestTransactionRetireEntity),
            //	new Config("/stages/{0}/transactions/{1}/retire", string.Empty, 0)
            //},
            //{
            //	typeof(QuestTransactionDefeatEntity),
            //	new Config("/stages/{0}/transactions/{1}/defeat", string.Empty, 0)
            //},
            //{
            //	typeof(EventTransactionCreateEntity),
            //	new Config("/special/stages/{0}/transactions", string.Empty, 0)
            //},
            //{
            //	typeof(EventTransactionUpdateEntity),
            //	new Config("/special/stages/{0}/transactions/{1}", string.Empty, 0)
            //},
            //{
            //	typeof(EventTransactionResultEntity),
            //	new Config("/special/stages/{0}/transactions/{1}/result", string.Empty, 0)
            //},
            //{
            //	typeof(EventTransactionRetireEntity),
            //	new Config("/special/stages/{0}/transactions/{1}/retire", string.Empty, 0)
            //},
            //{
            //	typeof(EventTransactionDefeatEntity),
            //	new Config("/special/stages/{0}/transactions/{1}/defeat", string.Empty, 0)
            //},
            //{
            //	typeof(WeekdayStaminaRecoveryTransactionCreate),
            //	new Config("/billing_point_shop/weekday_stamina_recovery/transactions", string.Empty, 0)
            //},
            //{
            //	typeof(WeekdayStaminaRecoveryTransactionUpdate),
            //	new Config("/billing_point_shop/weekday_stamina_recovery/transactions/{0}", string.Empty, 0)
            //},
            {
                typeof(CharacterFamiliarityEntity),
                new Config("/my/character_familiarities", "GET")
            },
            {
                typeof(GameResourceVersionEntity),
                new Config("/resource_versions/{type}", "GET")
            },
            {
                typeof(ScenarioResourceVersionEntity),
                new Config("/resource_versions/scenario/{scenario_id}", "GET")
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
                new Config("/requirement_version", "GET")
            },
            {
                typeof(PushTokenEntity),
                new Config("/my/push_token", "POST")
            },
            //{
            //	typeof(DeletePushTokenEnity),
            //	new Config("/my/push_token/removal", string.Empty, 0)
            //},
            //{
            //	typeof(CampaignEntity),
            //	new Config("/campaigns/{0}/entry", string.Empty, 0)
            //},
            //{
            //	typeof(ResponseErrorEntity),
            //	new Config("http://toybox.kaeru-the-frog.xyz/404.html", string.Empty, 0)
            //},
            //{
            //	typeof(BattleContinueConfirmEntity),
            //	new Config("/battle/transaction/validate", string.Empty, 0)
            //},
            //{
            //	typeof(BattleContinueEntity),
            //	new Config("/battle/continue/transactions", string.Empty, 0)
            //},
            //{
            //	typeof(BattleContinueInfoEntity),
            //	new Config("/battle/continue/transactions/{0}", string.Empty, 0)
            //},

            // The following entities don't have mono code!
            {
                typeof(UpdateClickCountsEntity),
                new Config("/click_counts/update_click_counts", "POST")
            },
            {
                typeof(EventItemsEntity),
                new Config("/my/event_items", "GET")
            },
            {
                typeof(PopupEntity),
                new Config("/popups", "GET")
            },
            {
                typeof(BuffsEntity),
                new Config("/my/buffs", "GET")
            },
            {
                typeof(AutoClearTicketsEntity),
                new Config("/my/auto_clear_tickets", "GET")
            },
            {
                typeof(CharacterTitleItemsEntity),
                new Config("/my/character_title_items", "POST")
            }
        };
    }

    public struct Config
    {
        public Config(string apiPath, params string[] httpMethods)
        {
            this.apiPath = apiPath;
            this.httpMethods = httpMethods;
        }

        public readonly string apiPath;
        public readonly string[] httpMethods;
    }
}