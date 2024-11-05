using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Positions
    /// </summary>
    public record WhiteBitPositionsUpdate
    {
        /// <summary>
        /// Total
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }
        /// <summary>
        /// Records
        /// </summary>
        [JsonPropertyName("records")]
        public IEnumerable<WhiteBitPositionUpdate> Records { get; set; } = Array.Empty<WhiteBitPositionUpdate>();
    }

    /// <summary>
    /// Position info
    /// </summary>
    public record WhiteBitPositionUpdate
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Create time
        /// </summary>
        [JsonPropertyName("ctime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Update time
        /// </summary>
        [JsonPropertyName("mtime")]
        public DateTime updateTime { get; set; }
        /// <summary>
        /// Position quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Position value
        /// </summary>
        [JsonPropertyName("amount_in_money")]
        public decimal Value { get; set; }
        /// <summary>
        /// Base price
        /// </summary>
        [JsonPropertyName("base_price")]
        public decimal BasePrice { get; set; }
        /// <summary>
        /// Profit and loss
        /// </summary>
        [JsonPropertyName("pnl")]
        public decimal UnrealizedPnl { get; set; }
        /// <summary>
        /// Liquidation price
        /// </summary>
        [JsonPropertyName("liq_price")]
        public decimal LiquidationPrice { get; set; }
        /// <summary>
        /// Liquidation stage
        /// </summary>
        [JsonPropertyName("liq_stage")]
        public string? LiquidationStage { get; set; }
        /// <summary>
        /// Unrealized funding
        /// </summary>
        [JsonPropertyName("unrealized_funding")]
        public decimal UnrealizedFunding { get; set; }
        /// <summary>
        /// Funding
        /// </summary>
        [JsonPropertyName("funding")]
        public decimal Funding { get; set; }
        /// <summary>
        /// Margin
        /// </summary>
        [JsonPropertyName("margin")]
        public decimal Margin { get; set; }
        /// <summary>
        /// Free margin
        /// </summary>
        [JsonPropertyName("free_margin")]
        public decimal FreeMargin { get; set; }
        /// <summary>
        /// Realized profit and loss
        /// </summary>
        [JsonPropertyName("realized_pnl")]
        public decimal RealizedPnl { get; set; }
    }


}
