using System;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Funding history
    /// </summary>
    public record WhiteBitFundingHistory
    {
        /// <summary>
        /// ["<c>fundingTime</c>"] Funding time
        /// </summary>
        [JsonPropertyName("fundingTime")]
        public DateTime FundingTime { get; set; }
        /// <summary>
        /// ["<c>fundingRate</c>"] Funding rate
        /// </summary>
        [JsonPropertyName("fundingRate")]
        public decimal FundingRate { get; set; }
        /// <summary>
        /// ["<c>settlementPrice</c>"] Settlement price
        /// </summary>
        [JsonPropertyName("settlementPrice")]
        public decimal SettlePrice { get; set; }
        /// <summary>
        /// ["<c>rateCalculatedTime</c>"] Funding rate calculation time
        /// </summary>
        [JsonPropertyName("rateCalculatedTime")]
        public DateTime FundingCalcTime { get; set; }
        /// <summary>
        /// ["<c>market</c>"] Symbol name
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
    }
}
