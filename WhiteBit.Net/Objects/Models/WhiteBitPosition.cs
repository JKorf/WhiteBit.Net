using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Position info
    /// </summary>
    [SerializationModel]
    public record WhiteBitPosition
    {
        /// <summary>
        /// ["<c>positionId</c>"] Position id
        /// </summary>
        [JsonPropertyName("positionId")]
        public long PositionId { get; set; }
        [JsonInclude, JsonPropertyName("id")]
        internal long PositionIdInt { get => PositionId; set => PositionId = value; }
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>openDate</c>"] Open timestamp
        /// </summary>
        [JsonPropertyName("openDate")]
        public DateTime OpenTime { get; set; }
        [JsonInclude, JsonPropertyName("ctime")]
        internal DateTime OpenTimeInt { get => OpenTime; set => OpenTime = value; }
        /// <summary>
        /// ["<c>modifyDate</c>"] Update timestamp
        /// </summary>
        [JsonPropertyName("modifyDate")]
        public DateTime UpdateTime { get; set; }
        [JsonInclude, JsonPropertyName("mtime")]
        internal DateTime UpdateTimeInt { get => UpdateTime; set => UpdateTime = value; }
        /// <summary>
        /// ["<c>amount</c>"] Quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>amount_in_money</c>"] Position value
        /// </summary>
        [JsonPropertyName("amount_in_money")]
        public decimal? Value { get; set; }
        /// <summary>
        /// ["<c>basePrice</c>"] Base price
        /// </summary>
        [JsonPropertyName("basePrice")]
        public decimal? BasePrice { get; set; }
        [JsonInclude, JsonPropertyName("base_price")]
        internal decimal? BasePriceInt { get => BasePrice; set => BasePrice = value; }
        /// <summary>
        /// ["<c>liquidationPrice</c>"] Liquidation price
        /// </summary>
        [JsonPropertyName("liquidationPrice")]
        public decimal? LiquidationPrice { get; set; }
        [JsonInclude, JsonPropertyName("liq_price")]
        internal decimal? LiquidationPriceInt { get => LiquidationPrice; set => LiquidationPrice = value; }
        /// <summary>
        /// ["<c>liquidationState</c>"] Liquidation status
        /// </summary>
        [JsonPropertyName("liquidationState")]
        public LiquidationStatus? LiquidationStatus { get; set; }
        [JsonInclude, JsonPropertyName("liq_stage")]
        internal LiquidationStatus? LiquidationStatusInt { get => LiquidationStatus; set => LiquidationStatus = value; }
        /// <summary>
        /// ["<c>pnl</c>"] Profit and loss
        /// </summary>
        [JsonPropertyName("pnl")]
        public decimal? Pnl { get; set; }
        /// <summary>
        /// ["<c>pnlPercent</c>"] Profit and loss percentage
        /// </summary>
        [JsonPropertyName("pnlPercent")]
        public decimal? PnlPercent { get; set; }
        /// <summary>
        /// ["<c>margin</c>"] Amount of funds in open position
        /// </summary>
        [JsonPropertyName("margin")]
        public decimal Margin { get; set; }
        /// <summary>
        /// ["<c>freeMargin</c>"] Free funds
        /// </summary>
        [JsonPropertyName("freeMargin")]
        public decimal FreeMargin { get; set; }
        [JsonInclude, JsonPropertyName("free_margin")]
        internal decimal FreeMarginInt { get => FreeMargin; set => FreeMargin = value; }
        /// <summary>
        /// ["<c>funding</c>"] Funding
        /// </summary>
        [JsonPropertyName("funding")]
        public decimal Funding { get; set; }
        /// <summary>
        /// ["<c>unrealizedFunding</c>"] Unrealized funding
        /// </summary>
        [JsonPropertyName("unrealizedFunding")]
        public decimal UnrealizedFunding { get; set; }
        [JsonInclude, JsonPropertyName("unrealized_funding")]
        internal decimal UnrealizedFundingInt { get => UnrealizedFunding; set => UnrealizedFunding = value; }
        /// <summary>
        /// ["<c>realized_pnl</c>"] Realized profit and loss
        /// </summary>
        [JsonPropertyName("realized_pnl")]
        public decimal? RealizedPnl { get; set; }
        /// <summary>
        /// ["<c>position_side</c>"] Position side
        /// </summary>
        [JsonPropertyName("position_side")]
        public PositionSide PositionSide { get; set; }
        /// <summary>
        /// ["<c>tpsl</c>"] Info on attached TP / SL orders for the position. Not available in websocket updates
        /// </summary>
        [JsonPropertyName("tpsl")]
        public WhiteBitPositionTpSl? TpSl { get; set; }
    }

    /// <summary>
    /// Info on TakeProfit/StopLoss attached orders for a position
    /// </summary>
    [SerializationModel]
    public record WhiteBitPositionTpSl
    {
        /// <summary>
        /// ["<c>takeProfitId</c>"] Take profit order id
        /// </summary>
        [JsonPropertyName("takeProfitId")]
        public long? TakeProfitId { get; set; }
        /// <summary>
        /// ["<c>takeProfit</c>"] Take profit price
        /// </summary>
        [JsonPropertyName("takeProfit")]
        public decimal? TakeProfitPrice { get; set; }
        /// <summary>
        /// ["<c>stopLossId</c>"] Stop loss order id
        /// </summary>
        [JsonPropertyName("stopLossId")]
        public long? StopLossId { get; set; }
        /// <summary>
        /// ["<c>stopLoss</c>"] Stop loss price
        /// </summary>
        [JsonPropertyName("stopLoss")]
        public decimal? StopLossPrice { get; set; }
    }
}
