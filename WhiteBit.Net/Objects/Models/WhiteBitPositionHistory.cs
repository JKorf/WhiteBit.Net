using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// 
    /// </summary>
    public record WhiteBitPositionHistory
    {
        /// <summary>
        /// Position id
        /// </summary>
        [JsonPropertyName("positionId")]
        public long PositionId { get; set; }
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Open time
        /// </summary>
        [JsonPropertyName("openDate")]
        public DateTime OpenTime { get; set; }
        /// <summary>
        /// Update time
        /// </summary>
        [JsonPropertyName("modifyDate")]
        public DateTime ModifyDate { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Base price
        /// </summary>
        [JsonPropertyName("basePrice")]
        public decimal BasePrice { get; set; }
        /// <summary>
        /// Realized funding
        /// </summary>
        [JsonPropertyName("realizedFunding")]
        public decimal RealizedFunding { get; set; }
        /// <summary>
        /// Liquidation price
        /// </summary>
        [JsonPropertyName("liquidationPrice")]
        public string? LiquidationPrice { get; set; }
        /// <summary>
        /// Liquidation status
        /// </summary>
        [JsonPropertyName("liquidationState")]
        public LiquidationStatus? LiquidationStatus { get; set; }
        /// <summary>
        /// Order detail
        /// </summary>
        [JsonPropertyName("orderDetail")]
        public WhiteBitPositionHistoryOrder OrderDetail { get; set; } = null!;
    }

    /// <summary>
    /// 
    /// </summary>
    public record WhiteBitPositionHistoryOrder
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        /// <summary>
        /// Trade quantity
        /// </summary>
        [JsonPropertyName("tradeAmount")]
        public decimal TradeQuantity { get; set; }
        /// <summary>
        /// Base price
        /// </summary>
        [JsonPropertyName("basePrice")]
        public decimal BasePrice { get; set; }
        /// <summary>
        /// Trade fee
        /// </summary>
        [JsonPropertyName("tradeFee")]
        public decimal TradeFee { get; set; }
        /// <summary>
        /// Funding fee
        /// </summary>
        [JsonPropertyName("fundingFee")]
        public decimal? FundingFee { get; set; }
        /// <summary>
        /// Realized profit and loss
        /// </summary>
        [JsonPropertyName("realizedPnl")]
        public decimal? RealizedPnl { get; set; }
    }


}
