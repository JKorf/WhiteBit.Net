using System;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Represents a paginated collection of funding history records for a WhiteBit account.
    /// </summary>
    /// <remarks>This record provides details about the pagination parameters, including the offset and limit,
    /// as well as the set of funding history entries. Use this type to retrieve and navigate through funding histories
    /// efficiently when working with large datasets.</remarks>
    public record WhiteBitAccountFundingHistories
    {
        /// <summary>
        /// Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        /// <summary>
        /// Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        /// <summary>
        /// Records
        /// </summary>
        [JsonPropertyName("records")]
        public WhiteBitAccountFundingHistory[] Records { get; set; } = [];
    }

    /// <summary>
    /// The history of realized funding payments for an account.
    /// </summary>
    public record WhiteBitAccountFundingHistory
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
        /// Funding amount
        /// </summary>
        [JsonPropertyName("fundingAmount")]
        public decimal FundingAmount { get; set; }
        /// <summary>
        /// Position amount
        /// </summary>
        [JsonPropertyName("positionAmount")]
        public decimal PositionAmount { get; set; }

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
