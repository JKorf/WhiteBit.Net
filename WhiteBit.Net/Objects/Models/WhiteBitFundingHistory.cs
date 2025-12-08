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
        /// Funding time
        /// </summary>
        [JsonPropertyName("fundingTime")]
        public DateTime FundingTime { get; set; }
        /// <summary>
        /// Funding rate
        /// </summary>
        [JsonPropertyName("fundingRate")]
        public decimal FundingRate { get; set; }
        /// <summary>
        /// Settlement price
        /// </summary>
        [JsonPropertyName("settlementPrice")]
        public decimal SettlePrice { get; set; }
        /// <summary>
        /// Funding rate calculation time
        /// </summary>
        [JsonPropertyName("rateCalculatedTime")]
        public DateTime FundingCalcTime { get; set; }
        /// <summary>
        /// Symbol name
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
    }
}
