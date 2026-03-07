using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Account summary
    /// </summary>
    [SerializationModel]
    public record WhiteBitCollateralAccountSummary
    {
        /// <summary>
        /// ["<c>equity</c>"] Total equity of collateral balance including lending funds in USDT
        /// </summary>
        [JsonPropertyName("equity")]
        public decimal Equity { get; set; }
        /// <summary>
        /// ["<c>margin</c>"] Amount of funds in open position USDT
        /// </summary>
        [JsonPropertyName("margin")]
        public decimal Margin { get; set; }
        /// <summary>
        /// ["<c>freeMargin</c>"] Free funds for trading
        /// </summary>
        [JsonPropertyName("freeMargin")]
        public decimal FreeMargin { get; set; }
        /// <summary>
        /// ["<c>unrealizedFunding</c>"] Funding that will be paid on next position stage change (order, liquidation, etc)
        /// </summary>
        [JsonPropertyName("unrealizedFunding")]
        public decimal UnrealizedFunding { get; set; }
        /// <summary>
        /// ["<c>pnl</c>"] Current profit and loss in USDT
        /// </summary>
        [JsonPropertyName("pnl")]
        public decimal Pnl { get; set; }
        /// <summary>
        /// ["<c>leverage</c>"] Current leverage of account which affect amount of lending funds
        /// </summary>
        [JsonPropertyName("leverage")]
        public decimal Leverage { get; set; }
        /// <summary>
        /// ["<c>marginFraction</c>"] Margin fraction
        /// </summary>
        [JsonPropertyName("marginFraction")]
        public decimal MarginFraction { get; set; }
        /// <summary>
        /// ["<c>maintenanceMarginFraction</c>"] Maintenance margin fraction
        /// </summary>
        [JsonPropertyName("maintenanceMarginFraction")]
        public decimal MaintenanceMarginFraction { get; set; }
    }


}
