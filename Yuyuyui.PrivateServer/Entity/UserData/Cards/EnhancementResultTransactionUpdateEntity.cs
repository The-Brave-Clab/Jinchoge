using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer
{
    public class EnhancementResultTransactionUpdateEntity : BaseEntity<EnhancementResultTransactionUpdateEntity>
    {
        public EnhancementResultTransactionUpdateEntity(
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

            long cardId = long.Parse(GetPathParameter("card_id"));
            long transactionId = long.Parse(GetPathParameter("transaction_id"));

            // Ignored the request body since it's the same as the path parameter
            //Request request = Deserialize<Request>(requestBody)!;

            EnhancementTransaction transaction = EnhancementTransaction.Load(transactionId);

            bool infiniteItems = Config.Get().InGame.InfiniteItems;

            // Validate here?

            using var enhancementDb = new EnhancementContext();
            using var itemsDb = new ItemsContext();
            using var cardsDb = new CardsContext();

            // the target card data
            Card userCard = Card.Load(cardId);
            // the use item data
            EnhancementItem usedItem;
            if (infiniteItems)
            {
                // in infinite items mode, the id and the master_id are the same
                usedItem = itemsDb.EnhancementItems
                    .First(i => i.Id == transaction.createdWith.enhancement_item.id);
            }
            else
            {
                Item userItem = Item.Load(transaction.createdWith.enhancement_item.id);
                // cost the item
                userItem.quantity -= transaction.createdWith.enhancement_item.quantity;
                userItem.Save();
                usedItem = itemsDb.EnhancementItems
                    .First(i => i.Id == userItem.master_id);
            }

            // first, we pick a cooking character
            var masterCard = userCard.MasterData(cardsDb);
            long targetCharacterId = masterCard.CharacterId;
            var cookingCharacterDataSource = enhancementDb.NoodleCookingCharacters
                .Where(c => c.TargetCharacterId == targetCharacterId); // this won't be empty at all
            NoodleCookingCharacter cookingCharacterData;
            long cookingCharacterId;
            if (usedItem.AvailableCharacterId1 != null && usedItem.AvailableCharacterId2 != null)
            {
                // this is a character udon
                // get the specified cooking character first
                cookingCharacterId = targetCharacterId == usedItem.AvailableCharacterId1
                    ? usedItem.AvailableCharacterId2 ?? 1
                    : // default value won't be used
                    usedItem.AvailableCharacterId1 ?? 1;
                // get the specified udon
                cookingCharacterData =
                    cookingCharacterDataSource.First(c => c.CookingCharacterId == cookingCharacterId);
            }
            else
            {
                // randomly pick one
                cookingCharacterData = cookingCharacterDataSource.Random(c => c.Weight)!;
                // get the cooking character
                cookingCharacterId = cookingCharacterData.CookingCharacterId;
            }

            Utils.Log(string.Format(Resources.LOG_PS_CARD_ENHANCMENT_COOKING_CHARACTER,
                cookingCharacterData.CookingCharacterId, cookingCharacterData.TargetCharacterId));

            // this is the result cooking data
            NoodleCooking cookingData =
                enhancementDb.NoodleCookings
                    .Where(c => c.EnhancementItemId == usedItem.Id)
                    .FirstOrDefault(c =>
                        c.CharacterId == cookingCharacterData.CookingCharacterId) ??
                // if it is null, it's a character specific udon
                // we just pick one from the database since this criteria result in the same result
                enhancementDb.NoodleCookings
                    .First(c => c.CharacterId == cookingCharacterId);
            
            // next, we check if this is a big hit
            bool bigHit = Utils.ProbabilityCheck(cookingData.SpecialHitPercent / 100.0f);

            if (bigHit)
                Utils.Log(Resources.LOG_PS_CARD_ENHANCEMENT_BIG_HIT);

            // get the used udon pack
            long noodleId = bigHit ? cookingData.SpecialNoodleId : cookingData.NoodleId;
            Noodle noodleData = enhancementDb.Noodles
                .First(n => n.Id == noodleId);

            // calculate the stats
            // exp & level
            long gotExp = usedItem.Exp * noodleData.ExpCoefficient * transaction.createdWith.enhancement_item.quantity;
            userCard.GainExp(cardsDb, gotExp);

            // active skill
            using var skillsDb = new SkillsContext();
            float activeSkillLevelUpProbability = CalcUtil.CalcActiveEnhancementChance(
                skillsDb,
                usedItem,
                masterCard.ActiveSkillId ?? 0,
                userCard.active_skill_level,
                transaction.createdWith.enhancement_item.quantity);
            Utils.Log(string.Format(Resources.LOG_PS_CARD_ENHANCEMENT_ACTIVE_SKILL_LEVEL_UP_PROBABILITY,
                activeSkillLevelUpProbability * 100.0f));
            bool activeSkillLevelUp = Utils.ProbabilityCheck(activeSkillLevelUpProbability);
            if (activeSkillLevelUp)
                Utils.Log(Resources.LOG_PS_CARD_ENHANCEMENT_ACTIVE_SKILL_LEVEL_UP);
            
            // Overall Support Skill
            float supportSkillLevelUpProbability = CalcUtil.CalcSupportEnhancementChance(
                skillsDb,
                usedItem,
                masterCard.SupportSkill1Id ?? 0,
                usedItem.SupportSkillLevelCategory,
                userCard.support_skill_level,
                transaction.createdWith.enhancement_item.quantity);
            Utils.Log(string.Format(Resources.LOG_PS_CARD_ENHANCEMENT_SUPPORT_SKILL_LEVEL_UP_PROBABILITY,
                supportSkillLevelUpProbability * 100.0f));
            bool supportSkillLevelUp = Utils.ProbabilityCheck(supportSkillLevelUpProbability);
            if (supportSkillLevelUp)
                Utils.Log(Resources.LOG_PS_CARD_ENHANCEMENT_SUPPORT_SKILL_LEVEL_UP);
          
            // character familiarity
            CharacterFamiliarityWithAssist familiarity = player.GetCharacterFamiliarity(cookingCharacterData.CookingCharacterId,
                cookingCharacterData.TargetCharacterId);

            var gotFamiliarity = CharacterFamiliarityWithAssist.GetEnhancement(usedItem.Id) *
                                 noodleData.ExpCoefficient *
                                 transaction.createdWith.enhancement_item.quantity;
            var gotAssistLevel = usedItem.AssistLevelPotential *
                                 noodleData.ExpCoefficient *
                                 transaction.createdWith.enhancement_item.quantity;

            CharacterFamiliarityChangeWithAssist familiarityChange;
            using (var charactersDb = new CharactersContext())
                familiarityChange = familiarity.UpdateAndGetChange(charactersDb, gotFamiliarity, gotAssistLevel);

            // finally, update the user data for card and character familiarity
            if (activeSkillLevelUp)
                // We don't need to validate this since the client won't let us use a skill udon if level is maxed out
                userCard.active_skill_level += 1;
            
            if (supportSkillLevelUp)
                userCard.support_skill_level += 1;

            userCard.Save();

            Utils.Log(string.Format(Resources.LOG_PS_CARD_ENHANCEMENT_AFFINITY_INCREASE,
                familiarity.character_group, familiarityChange.familiarity - familiarityChange.before_familiarity));
            Utils.Log(string.Format(Resources.LOG_PS_CARD_ENHANCEMENT_AFFINITY_ASSIST_LEVEL_INCREASE,
                familiarity.character_group, familiarityChange.assist_level - familiarityChange.before_assist_level));
            player.Save();

            if (!infiniteItems)
            {
                long costMoney =
                    CalcUtil.CalcRequiredEnhancementMoney(transaction.createdWith.enhancement_item.quantity,
                        usedItem.CostCoefficient);
                player.data.money -= costMoney;
                player.Save();
            }

            IList<int> resultTitleItems = player.EnsureEligibleCardTitle(cardsDb, itemsDb);
            player.Save();

            Response responseObj = new()
            {
                card_enhancement = new()
                {
                    card = CardsEntity.Card.FromPlayerCardData(cardsDb, userCard),
                    big_hit = bigHit,
                    noodle_master_id = noodleId,
                    cooking_character_master_id = cookingCharacterData.CookingCharacterId,
                    target_character_master_id = cookingCharacterData.TargetCharacterId,
                    character_familiarity = familiarityChange,
                    cooking_message =
                        bigHit ? cookingCharacterData.CookingSpecialMessage : cookingCharacterData.CookingMessage,
                    target_message =
                        bigHit ? cookingCharacterData.TargetSpecialMessage : cookingCharacterData.TargetMessage,
                    cooking_message_voice_id =
                        bigHit
                            ? cookingCharacterData.CookingSpecialMessageVoiceId
                            : cookingCharacterData.CookingMessageVoiceId,
                    target_message_voice_id =
                        bigHit
                            ? cookingCharacterData.TargetSpecialMessageVoiceId
                            : cookingCharacterData.TargetMessageVoiceId,
                    room_item_number = 0, // TODO: what is this?
                    title_items = resultTitleItems
                }
            };

            // Finished transaction, remove it
            transaction.Delete();

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        // public class Request
        // {
        //     public long card_id { get; set; }
        //     public long transaction_id { get; set; }
        // }

        public class Response
        {
            public CardEnhancement card_enhancement { get; set; } = new();

            public class CardEnhancement
            {
                public CardsEntity.Card card { get; set; } = new();
                public bool big_hit { get; set; }
                public long noodle_master_id { get; set; }
                public long cooking_character_master_id { get; set; }
                public long target_character_master_id { get; set; }
                public CharacterFamiliarityChangeWithAssist character_familiarity { get; set; } = new();
                public string cooking_message { get; set; } = "";
                public string target_message { get; set; } = "";
                public string target_message_voice_id { get; set; } = "";
                public string cooking_message_voice_id { get; set; } = "";
                public int room_item_number { get; set; } // what is this?
                public IList<int> title_items { get; set; } = new List<int>(); // TODO: datatype not confirmed
            }
        }
    }
}