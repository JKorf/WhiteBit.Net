using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Account summary
    /// </summary>
    public record WhiteBitCollateralAccountSummary
    {
        /// <summary>
        /// Total equity of collateral balance including lending funds in USDT
        /// </summary>
        [JsonPropertyName("equity")]
        public decimal Equity { get; set; }
        /// <summary>
        /// Amount of funds in open position USDT
        /// </summary>
        [JsonPropertyName("margin")]
        public decimal Margin { get; set; }
        /// <summary>
        /// Free funds for trading
        /// </summary>
        [JsonPropertyName("freeMargin")]
        public decimal FreeMargin { get; set; }
        /// <summary>
        /// Funding that will be paid on next position stage change (order, liquidation, etc)
        /// </summary>
        [JsonPropertyName("unrealizedFunding")]
        public decimal UnrealizedFunding { get; set; }
        /// <summary>
        /// Current profit and loss in USDT
        /// </summary>
        [JsonPropertyName("pnl")]
        public decimal Pnl { get; set; }
        /// <summary>
        /// Current leverage of account which affect amount of lending funds
        /// </summary>
        [JsonPropertyName("leverage")]
        public decimal Leverage { get; set; }
        /// <summary>
        /// Margin fraction
        /// </summary>
        [JsonPropertyName("marginFraction")]
        public decimal MarginFraction { get; set; }
        /// <summary>
        /// Maintenance margin fraction
        /// </summary>
        [JsonPropertyName("maintenanceMarginFraction")]
        public decimal MaintenanceMarginFraction { get; set; }
    }


}
