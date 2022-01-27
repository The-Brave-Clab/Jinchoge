using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public static class CalcUtil
    {
        #region Card

        public static int CalcHitPointByLevel(int targetLevel, int minLevel, int maxLevel, int minValue,
            int maxValue,
            float growth, int potential, int levelMaxBonus, float potentialArgument)
        {
            int num = CalcParamByLevel(targetLevel, minLevel, maxLevel, minValue, maxValue, growth);
            if (targetLevel >= maxLevel)
            {
                num += levelMaxBonus;
            }

            double num2 = num * CalcPotentialCoefficient(potential, potentialArgument);
            return (int) (Math.Ceiling(num2) + 0.5f);
        }

        public static int CalcAttackByLevel(int targetLevel, int minLevel, int maxLevel, int minValue, int maxValue,
            float growth, int potential, int levelMaxBonus, float potentialArgument)
        {
            int num = CalcParamByLevel(targetLevel, minLevel, maxLevel, minValue, maxValue, growth);
            if (targetLevel >= maxLevel)
            {
                num += levelMaxBonus;
            }

            double num2 = num * CalcPotentialCoefficient(potential, potentialArgument);
            return (int) (Math.Ceiling(num2) + 0.5f);
        }
        
        public static CardLevel GetLevelFromExp(int levelCategory, long exp)
        {
            using var cardsDb = new CardsContext();
            IEnumerable<CardLevel> source = cardsDb.CardLevels
                .Where(i => i.LevelCategory == levelCategory); // all level data in current category
            CardLevel? masterCardLevelData = source.FirstOrDefault(i => i.MaxExp >= exp); // find one
            if (masterCardLevelData != null) // if found
            {
                return masterCardLevelData;
            }
            return source.Last(); // if not found, return the highest level in current category
        }
        
        public static CardLevel GetExpFromLevel(int levelCategory, int level)
        {
            using var cardsDb = new CardsContext();
            IEnumerable<CardLevel> source = cardsDb.CardLevels
                .Where(i => i.LevelCategory == levelCategory);
            return source.First(i => i.Level == level);
        }
        
        #endregion

        #region Enhancement
        
        public static float CalcActiveEnhancementChance(EnhancementItem item, long skillId, int level, int count)
        {
            using var skillsDb = new SkillsContext();
            ActiveSkillComplete? activeSkillData = skillsDb.ActiveSkills
                .FirstOrDefault(i => i.Id == skillId);
            if (activeSkillData == null)
            {
                return 0f;
            }
            int levelCategory = activeSkillData.LevelCategory ?? 0; // those with null category are enemy skills
            ActiveSkillLevel? masterActiveSkillLevelData = skillsDb.ActiveSkillLevels
                .FirstOrDefault(arg => arg.LevelCategoy == levelCategory && arg.Level == level);
            if (masterActiveSkillLevelData == null || masterActiveSkillLevelData.LevelUpParam == null)
            {
                return 0f;
            }
            return item.ActiveSkillLevelPotential * count / (float)masterActiveSkillLevelData.LevelUpParam;
        }
        
        public static float CalcSupportEnhancementChance(
            EnhancementItem item, long skillId, int levelCategory, int level, int count)
        {
            using var skillsDb = new SkillsContext();
            PassiveSkill? passiveSkillData = skillsDb.PassiveSkills
                .FirstOrDefault(i => i.Id == skillId);
            if (passiveSkillData == null)
            {
                return 0f;
            }
            SupportSkillLevel? masterSupportSkillData = skillsDb.SupportSkillLevels
                .FirstOrDefault(arg => arg.SupportSkillLevelCategory == levelCategory && arg.Level == level);
            if (masterSupportSkillData == null || masterSupportSkillData.LevelUpParam == null)
            {
                return 0f;
            }
            return item.SupportSkillLevelPotential * count / (float)masterSupportSkillData.LevelUpParam;
        }
        
        public static int CalcRequiredEnhancementMoney(int useUdonNum, double costCoefficient)
        {
            return (int)Math.Floor(500 * costCoefficient) * useUdonNum; // the source code is not like this. why?
        }

        #endregion

        #region CharacterFamilarity
        
        public static int CalcAddedFamiliarityAndAssist(float original, 
            float assistBonus, double familiarityCoefficient)
        {
            int result = (int) Math.Floor((float) ((original + assistBonus) * familiarityCoefficient));
            return result;
        }
        
        public static int AssistLevelHitPointBonus(int assistLevel)
        {
            float num = 200f;
            float p = 0.3f;
            float num2 = 2f;
            float num3 = 10000f;
            float num4 = num * (float)Math.Pow((float)assistLevel, p);
            float num5 = assistLevel > num3 ? num3 * 2f : assistLevel * num2;
            return (int) Math.Floor(num4 + num5);
        }

        public static int AssistLevelAttackBonus(int assistLevel)
        {
            float num = 100f;
            float p = 0.3f;
            return (int) Math.Floor(num * Math.Pow((float)assistLevel, p));
        }
        
        public static FamiliarityLevel GetFamiliarityRankFromExp(long exp)
        {
            using var charactersDb = new CharactersContext();
            IEnumerable<FamiliarityLevel> source = charactersDb.FamiliarityLevels;
            FamiliarityLevel? masterFamiliarityLevelData = source.FirstOrDefault(i => i.MaxExp >= exp);
            if (masterFamiliarityLevelData != null) // if found
            {
                return masterFamiliarityLevelData;
            }
            return source.Last(); // if not found, return the highest level in current category
        }
        
        public static FamiliarityLevel GetExpFromFamiliarityRank(int level)
        {
            using var charactersDb = new CharactersContext();
            IEnumerable<FamiliarityLevel> source = charactersDb.FamiliarityLevels;
            return source.First(i => i.Level == level);
        }

        #endregion

        #region GeneralCalc

        public static double CalcPotentialCoefficient(int potential, float potentialArgument)
        {
            if (potential != 0)
            {
                return Math.Pow(potential + 1, potentialArgument);
            }

            return 1.0;
        }

        public static int CalcParamByLevel(int targetLevel, int minLevel, int maxLevel, int minValue, int maxValue,
            float growth)
        {
            float num = (targetLevel - minLevel) / (float) (maxLevel - minLevel);
            num = Math.Clamp(num, 0f, 1f);
            return minValue + (int) ((maxValue - minValue) * Math.Pow(num, growth));
        }
        
        public static bool IsSanchoDay()
        {
            int day = DateTime.Now.Day;
            return day is 3 or 13 or 23;
        }

        #endregion
    }


    public static class GrowthKind
    {
        public static float GetValue(int kind)
        {
            return Values[kind];
        }

        private static readonly float[] Values = {0f, 0.7f, 0.85f, 1f, 1.25f, 1.5f};
    }
}