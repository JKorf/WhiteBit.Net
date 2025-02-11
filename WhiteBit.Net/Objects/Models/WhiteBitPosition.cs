using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Position info
    /// </summary>
    public record WhiteBitPosition
    {
        /// <summary>
        /// Position id
        /// </summary>
        [JsonPropertyName("positionId")]
        public long PositionId { get; set; }
        [JsonInclude, JsonPropertyName("id")]
        internal long PositionIdInt { get => PositionId; set => PositionId = value; }
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Open timestamp
        /// </summary>
        [JsonPropertyName("openDate")]
        public DateTime OpenTime { get; set; }
        [JsonInclude, JsonPropertyName("ctime")]
        internal DateTime OpenTimeInt { get => OpenTime; set => OpenTime = value; }
        /// <summary>
        /// Update timestamp
        /// </summary>
        [JsonPropertyName("modifyDate")]
        public DateTime UpdateTime { get; set; }
        [JsonInclude, JsonPropertyName("mtime")]
        internal DateTime UpdateTimeInt { get => UpdateTime; set => UpdateTime = value; }
        /// <summary>
        /// Quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Position value
        /// </summary>
        [JsonPropertyName("amount_in_money")]
        public decimal? Value { get; set; }
        /// <summary>
        /// Base price
        /// </summary>
        [JsonPropertyName("basePrice")]
        public decimal? BasePrice { get; set; }
        [JsonInclude, JsonPropertyName("base_price")]
        internal decimal? BasePriceInt { get => BasePrice; set => BasePrice = value; }
        /// <summary>
        /// Liquidation price
        /// </summary>
        [JsonPropertyName("liquidationPrice")]
        public decimal? LiquidationPrice { get; set; }
        [JsonInclude, JsonPropertyName("liq_price")]
        internal decimal? LiquidationPriceInt { get => LiquidationPrice; set => LiquidationPrice = value; }
        /// <summary>
        /// Liquidation status
        /// </summary>
        [JsonPropertyName("liquidationState")]
        public LiquidationStatus? LiquidationStatus { get; set; }
        [JsonInclude, JsonPropertyName("liq_stage")]
        internal LiquidationStatus? LiquidationStatusInt { get => LiquidationStatus; set => LiquidationStatus = value; }
        /// <summary>
        /// Profit and loss
        /// </summary>
        [JsonPropertyName("pnl")]
        public decimal? Pnl { get; set; }
        /// <summary>
        /// Profit and loss percentage
        /// </summary>
        [JsonPropertyName("pnlPercent")]
        public decimal? PnlPercent { get; set; }
        /// <summary>
        /// Amount of funds in open position
        /// </summary>
        [JsonPropertyName("margin")]
        public decimal Margin { get; set; }
        /// <summary>
        /// Free funds
        /// </summary>
        [JsonPropertyName("freeMargin")]
        public decimal FreeMargin { get; set; }
        [JsonInclude, JsonPropertyName("free_margin")]
        internal decimal FreeMarginInt { get => FreeMargin; set => FreeMargin = value; }
        /// <summary>
        /// Funding
        /// </summary>
        [JsonPropertyName("funding")]
        public decimal Funding { get; set; }
        /// <summary>
        /// Unrealized funding
        /// </summary>
        [JsonPropertyName("unrealizedFunding")]
        public decimal UnrealizedFunding { get; set; }
        [JsonInclude, JsonPropertyName("unrealized_funding")]
        internal decimal UnrealizedFundingInt { get => UnrealizedFunding; set => UnrealizedFunding = value; }
        /// <summary>
        /// Realized profit and loss
        /// </summary>
        [JsonPropertyName("realized_pnl")]
        public decimal? RealizedPnl { get; set; }
        /// <summary>
        /// Info on attached TP / SL orders for the position. Not available in websocket updates
        /// </summary>
        [JsonPropertyName("tpsl")]
        public WhiteBitPositionTpSl? TpSl { get; set; }
    }

    /// <summary>
    /// Info on TakeProfit/StopLoss attached orders for a position
    /// </summary>
    public record WhiteBitPositionTpSl
    {
        /// <summary>
        /// Take profit order id
        /// </summary>
        [JsonPropertyName("takeProfitId")]
        public long? TakeProfitId { get; set; }
        /// <summary>
        /// Take profit price
        /// </summary>
        [JsonPropertyName("takeProfit")]
        public decimal? TakeProfitPrice { get; set; }
        /// <summary>
        /// Stop loss order id
        /// </summary>
        [JsonPropertyName("stopLossId")]
        public long? StopLossId { get; set; }
        /// <summary>
        /// Stop loss price
        /// </summary>
        [JsonPropertyName("stopLoss")]
        public decimal? StopLossPrice { get; set; }
    }
}
