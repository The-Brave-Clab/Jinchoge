using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class SpecialEpisodeDifficulty
    {
        public long Id { get; set; }
        public long SpecialEpisodeId { get; set; }
        public int Difficulty { get; set; }
        public string? ScoreCompletedReward1ContentType { get; set; } = null;
        public long? ScoreCompletedReward1ItemCategoryId { get; set; } = null;
        public long? ScoreCompletedReward1ContentId { get; set; } = null;
        public int? ScoreCompletedReward1Quantity { get; set; } = null;
        public string? ScoreCompletedReward2ContentType { get; set; } = null;
        public long? ScoreCompletedReward2ItemCategoryId { get; set; } = null;
        public long? ScoreCompletedReward2ContentId { get; set; } = null;
        public int? ScoreCompletedReward2Quantity { get; set; } = null;
        public string? ScoreCompletedReward3ContentType { get; set; } = null;
        public long? ScoreCompletedReward3ItemCategoryId { get; set; } = null;
        public long? ScoreCompletedReward3ContentId { get; set; } = null;
        public int? ScoreCompletedReward3Quantity { get; set; } = null;
    }
}