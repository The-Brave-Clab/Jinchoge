using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class EnhancementResultTransactionUpdateEntity : BaseEntity<EnhancementResultTransactionUpdateEntity>
    {
        public EnhancementResultTransactionUpdateEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();

            Utils.Log("Path parameters:");
            foreach (var pathParameter in pathParameters)
            {
                Utils.Log($"\t{pathParameter.Key} = {pathParameter.Value}");
            }

            long cardId = long.Parse(GetPathParameter("card_id"));
            long transactionId = long.Parse(GetPathParameter("transaction_id"));

            // Ignored the request body since it's the same as the path parameter
            //Request request = Deserialize<Request>(requestBody)!;

            EnhancementTransaction transaction = EnhancementTransaction.Load(transactionId);

            // Validate here?

            using var enhancementDb = new EnhancementContext();
            using var itemsDb = new ItemsContext();
            using var cardsDb = new CardsContext();

            // the target card data
            Card userCard = Card.Load(cardId);
            // the use item data
            Item userItem = Item.Load(transaction.createdWith.enhancement_item.id);
            EnhancementItem usedItem = itemsDb.EnhancementItems
                .First(i => i.Id == userItem.master_id);

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

            Utils.Log($"cooking character {cookingCharacterData.CookingCharacterId}" +
                      $" -> target character {cookingCharacterData.TargetCharacterId}");

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

            Utils.Log($"bigHit is {bigHit}");

            // get the used udon pack
            long noodleId = bigHit ? cookingData.SpecialNoodleId : cookingData.NoodleId;
            Noodle noodleData = enhancementDb.Noodles
                .First(n => n.Id == noodleId);

            // calculate the stats
            // exp & level
            long gotExp = usedItem.Exp * noodleData.ExpCoefficient * transaction.createdWith.enhancement_item.quantity;
            long newExpUncapped = userCard.exp + gotExp;
            CardLevel newLevelUncapped = CalcUtil.GetLevelFromExp(cardsDb, masterCard.LevelCategory, newExpUncapped);
            bool expOverflow = newLevelUncapped.Level >= masterCard.MaxLevel;
            int newLevel = expOverflow ? masterCard.MaxLevel : newLevelUncapped.Level;
            long newExp = expOverflow
                // default value only for casting from nullable, won't be used at all by theory
                ? CalcUtil.GetExpFromLevel(cardsDb, masterCard.LevelCategory, newLevel - 1).MaxExp + 1 ?? 0
                : newExpUncapped;
            // active skill
            using var skillsDb = new SkillsContext();
            float activeSkillLevelUpProbability = CalcUtil.CalcActiveEnhancementChance(
                skillsDb,
                usedItem,
                masterCard.ActiveSkillId ?? 0,
                userCard.active_skill_level,
                transaction.createdWith.enhancement_item.quantity);
            Utils.Log($"active skill level up probability is {activeSkillLevelUpProbability * 100.0f}%");
            bool activeSkillLevelUp = Utils.ProbabilityCheck(activeSkillLevelUpProbability);
            Utils.Log($"active skill level up {activeSkillLevelUp}");
            // support skill 1
            float supportSkill1LevelUpProbability = CalcUtil.CalcSupportEnhancementChance(
                skillsDb,
                usedItem,
                masterCard.SupportSkill1Id ?? 0,
                usedItem.SupportSkillLevelCategory,
                userCard.support_skill_1_level,
                transaction.createdWith.enhancement_item.quantity);
            Utils.Log($"support skill 1 level up probability is {supportSkill1LevelUpProbability * 100.0f}%");
            bool supportSkill1LevelUp = Utils.ProbabilityCheck(supportSkill1LevelUpProbability);
            Utils.Log($"support skill 1 level up {supportSkill1LevelUp}");
            // support skill 2
            float supportSkill2LevelUpProbability = CalcUtil.CalcSupportEnhancementChance(
                skillsDb,
                usedItem,
                masterCard.SupportSkill1Id ?? 0,
                usedItem.SupportSkillLevelCategory,
                userCard.support_skill_2_level,
                transaction.createdWith.enhancement_item.quantity);
            Utils.Log($"support skill 2 level up probability is {supportSkill2LevelUpProbability * 100.0f}%");
            bool supportSkill2LevelUp = Utils.ProbabilityCheck(supportSkill2LevelUpProbability);
            Utils.Log($"support skill 2 level up {supportSkill2LevelUp}");
            // character familiarity
            CharacterFamiliarity familiarity = player.GetCharacterFamiliarity(cookingCharacterData.CookingCharacterId,
                cookingCharacterData.TargetCharacterId);
            var beforeRank = familiarity.rank;
            var beforeFamiliarity = familiarity.familiarity;
            var beforeAssistLevel = familiarity.assist_level;

            var gotFamiliarity = CharacterFamiliarity.GetEnhancement(usedItem.Id) *
                                 noodleData.ExpCoefficient *
                                 transaction.createdWith.enhancement_item.quantity;
            var gotAssistLevel = usedItem.AssistLevelPotential *
                                 noodleData.ExpCoefficient *
                                 transaction.createdWith.enhancement_item.quantity;

            var newFamiliarityUncapped = beforeFamiliarity + gotFamiliarity;

            using var charactersDb = new CharactersContext();
            FamiliarityLevel newRankUncapped = CalcUtil.GetFamiliarityRankFromExp(charactersDb, newFamiliarityUncapped);
            bool familiarityOverflow = newRankUncapped.Level >= 25;
            int newRank = familiarityOverflow ? 25 : newRankUncapped.Level;
            int newFamiliarity = familiarityOverflow
                ? CalcUtil.GetExpFromFamiliarityRank(charactersDb, newRank - 1).MaxExp + 1 ?? 0
                : newFamiliarityUncapped;

            int newAssistLevel = beforeAssistLevel + gotAssistLevel; // will this overflow?

            // finally, update the user data for item, card and character familiarity
            userCard.exp = newExp;
            userCard.level = newLevel;
            if (activeSkillLevelUp)
                // We don't need to validate this since the client won't let us use a skill udon if level is maxed out
                userCard.active_skill_level += 1;
            if (supportSkill1LevelUp)
                userCard.support_skill_1_level += 1;
            if (supportSkill2LevelUp)
                userCard.support_skill_2_level += 1;
            userCard.Save();

            familiarity.rank = newRank;
            familiarity.familiarity = newFamiliarity;
            familiarity.assist_level = newAssistLevel;
            Utils.Log($"familiarity {familiarity.character_group} value +{newFamiliarity - beforeFamiliarity}");
            Utils.Log($"familiarity {familiarity.character_group} assist level +{gotAssistLevel}");
            player.Save();

            long costMoney =
                CalcUtil.CalcRequiredEnhancementMoney(transaction.createdWith.enhancement_item.quantity,
                    usedItem.CostCoefficient);
            player.data.money -= costMoney;
            player.Save();

            userItem.quantity -= transaction.createdWith.enhancement_item.quantity;
            userItem.Save();

            Response responseObj = new()
            {
                card_enhancement = new()
                {
                    card = CardsEntity.Card.FromPlayerCardData(cardsDb, userCard),
                    big_hit = bigHit,
                    noodle_master_id = noodleId,
                    cooking_character_master_id = cookingCharacterData.CookingCharacterId,
                    target_character_master_id = cookingCharacterData.TargetCharacterId,
                    character_familiarity = new()
                    {
                        character_group = familiarity.character_group,
                        familiarity = newFamiliarity,
                        rank = newRank,
                        before_familiarity = beforeFamiliarity,
                        before_rank = beforeRank,
                        assist_level = newAssistLevel,
                        before_assist_level = beforeAssistLevel
                    },
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
                    title_items = new List<int>() // TODO
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
                public CharacterFamiliarityChange character_familiarity { get; set; } = new();
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