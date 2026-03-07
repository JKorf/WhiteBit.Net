using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SerializationModel]
    public record WhiteBitPositionHistory
    {
        /// <summary>
        /// ["<c>positionId</c>"] Position id
        /// </summary>
        [JsonPropertyName("positionId")]
        public long PositionId { get; set; }
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>openDate</c>"] Open time
        /// </summary>
        [JsonPropertyName("openDate")]
        public DateTime OpenTime { get; set; }
        /// <summary>
        /// ["<c>modifyDate</c>"] Update time
        /// </summary>
        [JsonPropertyName("modifyDate")]
        public DateTime ModifyDate { get; set; }
        /// <summary>
        /// ["<c>amount</c>"] Quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>basePrice</c>"] Base price
        /// </summary>
        [JsonPropertyName("basePrice")]
        public decimal BasePrice { get; set; }
        /// <summary>
        /// ["<c>realizedFunding</c>"] Realized funding
        /// </summary>
        [JsonPropertyName("realizedFunding")]
        public decimal RealizedFunding { get; set; }
        /// <summary>
        /// ["<c>liquidationPrice</c>"] Liquidation price
        /// </summary>
        [JsonPropertyName("liquidationPrice")]
        public string? LiquidationPrice { get; set; }
        /// <summary>
        /// ["<c>liquidationState</c>"] Liquidation status
        /// </summary>
        [JsonPropertyName("liquidationState")]
        public LiquidationStatus? LiquidationStatus { get; set; }
        /// <summary>
        /// ["<c>orderDetail</c>"] Order detail
        /// </summary>
        [JsonPropertyName("orderDetail")]
        public WhiteBitPositionHistoryOrder OrderDetail { get; set; } = null!;
    }

    /// <summary>
    /// 
    /// </summary>
    [SerializationModel]
    public record WhiteBitPositionHistoryOrder
    {
        /// <summary>
        /// ["<c>id</c>"] Id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        /// <summary>
        /// ["<c>tradeAmount</c>"] Trade quantity
        /// </summary>
        [JsonPropertyName("tradeAmount")]
        public decimal TradeQuantity { get; set; }
        /// <summary>
        /// ["<c>basePrice</c>"] Base price
        /// </summary>
        [JsonPropertyName("basePrice")]
        public decimal BasePrice { get; set; }
        /// <summary>
        /// ["<c>tradeFee</c>"] Trade fee
        /// </summary>
        [JsonPropertyName("tradeFee")]
        public decimal TradeFee { get; set; }
        /// <summary>
        /// ["<c>fundingFee</c>"] Funding fee
        /// </summary>
        [JsonPropertyName("fundingFee")]
        public decimal? FundingFee { get; set; }
        /// <summary>
        /// ["<c>realizedPnl</c>"] Realized profit and loss
        /// </summary>
        [JsonPropertyName("realizedPnl")]
        public decimal? RealizedPnl { get; set; }
    }


}
