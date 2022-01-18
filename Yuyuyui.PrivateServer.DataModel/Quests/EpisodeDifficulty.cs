using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class EpisodeDifficulty
    {
        public long Id { get; set; }
        public long EpisodeId { get; set; }
        public int Difficulty { get; set; }
        public string? ScoreCompletedReward1ContentType { get; set; }
        public long? ScoreCompletedReward1ItemCategoryId { get; set; }
        public long? ScoreCompletedReward1ContentId { get; set; }
        public int? ScoreCompletedReward1Quantity { get; set; }
        public string? ScoreCompletedReward2ContentType { get; set; }
        public long? ScoreCompletedReward2ItemCategoryId { get; set; }
        public long? ScoreCompletedReward2ContentId { get; set; }
        public int? ScoreCompletedReward2Quantity { get; set; }
        public string? ScoreCompletedReward3ContentType { get; set; }
        public long? ScoreCompletedReward3ItemCategoryId { get; set; }
        public long? ScoreCompletedReward3ContentId { get; set; }
        public int? ScoreCompletedReward3Quantity { get; set; }
    }
}