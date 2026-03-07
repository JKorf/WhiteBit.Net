using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Mining reward history
    /// </summary>
    [SerializationModel]
    public record WhiteBitMiningRewards
    {
        /// <summary>
        /// ["<c>offset</c>"] Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public decimal Offset { get; set; }
        /// <summary>
        /// ["<c>limit</c>"] Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public decimal Limit { get; set; }
        /// <summary>
        /// ["<c>data</c>"] Data
        /// </summary>
        [JsonPropertyName("data")]
        public WhiteBitMiningReward[] Data { get; set; } = Array.Empty<WhiteBitMiningReward>();
    }

    /// <summary>
    /// Reward info
    /// </summary>
    [SerializationModel]
    public record WhiteBitMiningReward
    {
        /// <summary>
        /// ["<c>miningAccountName</c>"] Mining account name
        /// </summary>
        [JsonPropertyName("miningAccountName")]
        public string AccountName { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>totalReward</c>"] Total reward
        /// </summary>
        [JsonPropertyName("totalReward")]
        public decimal TotalReward { get; set; }
        /// <summary>
        /// ["<c>reward</c>"] Reward
        /// </summary>
        [JsonPropertyName("reward")]
        public decimal Reward { get; set; }
        /// <summary>
        /// ["<c>fee</c>"] Fee
        /// </summary>
        [JsonPropertyName("fee")]
        public decimal Fee { get; set; }
        /// <summary>
        /// ["<c>fppsRate</c>"] Fpps rate
        /// </summary>
        [JsonPropertyName("fppsRate")]
        public decimal FppsRate { get; set; }
        /// <summary>
        /// ["<c>hashRate</c>"] Hash rate
        /// </summary>
        [JsonPropertyName("hashRate")]
        public decimal HashRate { get; set; }
        /// <summary>
        /// ["<c>date</c>"] Timestamp
        /// </summary>
        [JsonPropertyName("date")]
        public DateTime Timestamp { get; set; }
    }


}
