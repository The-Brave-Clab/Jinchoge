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

        #endregion

        #region CharacterFamilarity

        public static Tuple<int, int> CalcAssistBonus(int assistLevel)
        {
            Tuple<int, int> result = new Tuple<int, int>(
                AssistLevelHitPointBonus(assistLevel), AssistLevelAttackBonus(assistLevel));
            return result;
        }

        public static int AssistLevelHitPointBonus(int assistLevel)
        {
            float num = 200f;
            float p = 0.3f;
            float num2 = 2f;
            float num3 = 10000f;
            float num4 = num * (float)Math.Pow((float)assistLevel, p);
            float num5 = ((float)assistLevel > num3) ? (num3 * 2f) : ((float)assistLevel * num2);
            return (int) Math.Floor(num4 + num5);
        }

        public static int AssistLevelAttackBonus(int assistLevel)
        {
            float num = 100f;
            float p = 0.3f;
            return (int) Math.Floor(num * Math.Pow((float)assistLevel, p));
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