using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Mining reward history
    /// </summary>
    public record WhiteBitMiningRewards
    {
        /// <summary>
        /// Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public decimal Offset { get; set; }
        /// <summary>
        /// Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public decimal Limit { get; set; }
        /// <summary>
        /// Data
        /// </summary>
        [JsonPropertyName("data")]
        public IEnumerable<WhiteBitMiningReward> Data { get; set; } = Array.Empty<WhiteBitMiningReward>();
    }

    /// <summary>
    /// Reward info
    /// </summary>
    public record WhiteBitMiningReward
    {
        /// <summary>
        /// Mining account name
        /// </summary>
        [JsonPropertyName("miningAccountName")]
        public string AccountName { get; set; } = string.Empty;
        /// <summary>
        /// Total reward
        /// </summary>
        [JsonPropertyName("totalReward")]
        public decimal TotalReward { get; set; }
        /// <summary>
        /// Reward
        /// </summary>
        [JsonPropertyName("reward")]
        public decimal Reward { get; set; }
        /// <summary>
        /// Fee
        /// </summary>
        [JsonPropertyName("fee")]
        public decimal Fee { get; set; }
        /// <summary>
        /// Fpps rate
        /// </summary>
        [JsonPropertyName("fppsRate")]
        public decimal FppsRate { get; set; }
        /// <summary>
        /// Hash rate
        /// </summary>
        [JsonPropertyName("hashRate")]
        public decimal HashRate { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("date")]
        public DateTime Timestamp { get; set; }
    }


}
