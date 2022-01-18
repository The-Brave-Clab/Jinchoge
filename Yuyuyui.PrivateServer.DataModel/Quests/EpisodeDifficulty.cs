using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class EpisodeDifficulty
    {
        public byte[]? Id { get; set; }
        public byte[]? EpisodeId { get; set; }
        public byte[]? Difficulty { get; set; }
        public byte[]? ScoreCompletedReward1ContentType { get; set; }
        public byte[]? ScoreCompletedReward1ItemCategoryId { get; set; }
        public byte[]? ScoreCompletedReward1ContentId { get; set; }
        public byte[]? ScoreCompletedReward1Quantity { get; set; }
        public byte[]? ScoreCompletedReward2ContentType { get; set; }
        public byte[]? ScoreCompletedReward2ItemCategoryId { get; set; }
        public byte[]? ScoreCompletedReward2ContentId { get; set; }
        public byte[]? ScoreCompletedReward2Quantity { get; set; }
        public byte[]? ScoreCompletedReward3ContentType { get; set; }
        public byte[]? ScoreCompletedReward3ItemCategoryId { get; set; }
        public byte[]? ScoreCompletedReward3ContentId { get; set; }
        public byte[]? ScoreCompletedReward3Quantity { get; set; }
    }
}
