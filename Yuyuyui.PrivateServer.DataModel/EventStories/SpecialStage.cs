using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SpecialStage
    {
        public long Id { get; set; }
        public long SpecialChapterId { get; set; }
        public long SpecialEpisodeId { get; set; }
        public int Number { get; set; }
        public int Kind { get; set; }
        public int Difficulty { get; set; }
        public int StaminaUsage { get; set; }
        public int CardExp { get; set; }
        public int Familiarity { get; set; }
        public int UserExp { get; set; }
        public long Money { get; set; }
        public int FriendPoint { get; set; } // All 50
        public int GuestFriendPoint { get; set; } // All 20
        public int ExchangePointRate { get; set; }
        public string? ScoreReward1ContentType { get; set; }
        public long? ScoreReward1ItemCategoryId { get; set; }
        public long? ScoreReward1ContentId { get; set; }
        public int? ScoreReward1Quantity { get; set; }
        public string? ScoreReward2ContentType { get; set; }
        public long? ScoreReward2ItemCategoryId { get; set; }
        public long? ScoreReward2ContentId { get; set; }
        public int? ScoreReward2Quantity { get; set; }
        public string? ScoreReward3ContentType { get; set; }
        public long? ScoreReward3ItemCategoryId { get; set; }
        public long? ScoreReward3ContentId { get; set; }
        public int? ScoreReward3Quantity { get; set; }
        public string? FinishedReward1ContentType { get; set; }
        public long? FinishedReward1ItemCategoryId { get; set; }
        public long? FinishedReward1ContentId { get; set; }
        public int? FinishedReward1Quantity { get; set; }
        public string? FinishedReward2ContentType { get; set; }
        public long? FinishedReward2ItemCategoryId { get; set; }
        public long? FinishedReward2ContentId { get; set; }
        public int? FinishedReward2Quantity { get; set; }
        public string? FinishedReward3ContentType { get; set; }
        public long? FinishedReward3ItemCategoryId { get; set; }
        public long? FinishedReward3ContentId { get; set; }
        public int? FinishedReward3Quantity { get; set; }
        public int? Element1 { get; set; }
        public int? Element2 { get; set; }
        public int? Element3 { get; set; }
        public string? DifficultyRating { get; set; } // should be int? but there is a thing 100~999 that prevents it
        public string? Title { get; set; } // "stage name" or a number
        public string Position { get; set; } // in format of "<int>,<int>"
        public long BattleBackground { get; set; } // is this id?
        public string BattleBgmNormal { get; set; }
        public string BattleBgmChance { get; set; }
        public string BattleBgmPinch { get; set; }
        public string BattleBgmBoss { get; set; }
        public string? ScenarioIdA { get; set; }
        public string? ScenarioIdB { get; set; }
        public long? BattleLimitTableId { get; set; }
        public int WeeklyScore { get; set; } // All 0
        public int AutoClear { get; set; } // 01 boolean
        public int? NoFriend { get; set; } // boolean, 1 for true, null for false
    }
}