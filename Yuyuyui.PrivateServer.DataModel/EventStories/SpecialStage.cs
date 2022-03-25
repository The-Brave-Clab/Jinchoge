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
        public string? ScoreReward1ContentType { get; set; } = null;
        public long? ScoreReward1ItemCategoryId { get; set; } = null;
        public long? ScoreReward1ContentId { get; set; } = null;
        public int? ScoreReward1Quantity { get; set; } = null;
        public string? ScoreReward2ContentType { get; set; } = null;
        public long? ScoreReward2ItemCategoryId { get; set; } = null;
        public long? ScoreReward2ContentId { get; set; } = null;
        public int? ScoreReward2Quantity { get; set; } = null;
        public string? ScoreReward3ContentType { get; set; } = null;
        public long? ScoreReward3ItemCategoryId { get; set; } = null;
        public long? ScoreReward3ContentId { get; set; } = null;
        public int? ScoreReward3Quantity { get; set; } = null;
        public string? FinishedReward1ContentType { get; set; } = null;
        public long? FinishedReward1ItemCategoryId { get; set; } = null;
        public long? FinishedReward1ContentId { get; set; } = null;
        public int? FinishedReward1Quantity { get; set; } = null;
        public string? FinishedReward2ContentType { get; set; } = null;
        public long? FinishedReward2ItemCategoryId { get; set; } = null;
        public long? FinishedReward2ContentId { get; set; } = null;
        public int? FinishedReward2Quantity { get; set; } = null;
        public string? FinishedReward3ContentType { get; set; } = null;
        public long? FinishedReward3ItemCategoryId { get; set; } = null;
        public long? FinishedReward3ContentId { get; set; } = null;
        public int? FinishedReward3Quantity { get; set; } = null;
        public int? Element1 { get; set; } = null;
        public int? Element2 { get; set; } = null;
        public int? Element3 { get; set; } = null;
        public string? DifficultyRating { get; set; } = null; // should be int? but there is a thing 100~999 that prevents it
        public string? Title { get; set; } = null; // "stage name" or a number
        public string Position { get; set; } = ""; // in format of "<int>,<int>"
        public long BattleBackground { get; set; } // is this id?
        public string BattleBgmNormal { get; set; } = "";
        public string BattleBgmChance { get; set; } = "";
        public string BattleBgmPinch { get; set; } = "";
        public string BattleBgmBoss { get; set; } = "";
        public string? ScenarioIdA { get; set; } = null;
        public string? ScenarioIdB { get; set; } = null;
        public long? BattleLimitTableId { get; set; } = null;
        public int WeeklyScore { get; set; } // All 0
        public int AutoClear { get; set; } // 01 boolean
        public int? NoFriend { get; set; } = null; // boolean, 1 for true, null for false
    }
}