using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Collateral order info
    /// </summary>
    public record WhiteBitCollateralOrder
    {
        /// <summary>
        /// Order id
        /// </summary>
        [JsonPropertyName("orderId")]
        public long OrderId { get; set; }
        /// <summary>
        /// Client order id
        /// </summary>
        [JsonPropertyName("clientOrderId")]
        public string? ClientOrderId { get; set; }
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Order side
        /// </summary>
        [JsonPropertyName("side")]
        public OrderSide OrderSide { get; set; }
        /// <summary>
        /// Order type
        /// </summary>
        [JsonPropertyName("type")]
        public OrderType OrderType { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Filled quantity in quote asset
        /// </summary>
        [JsonPropertyName("dealMoney")]
        public decimal QuoteQuantityFilled { get; set; }
        /// <summary>
        /// Filled quantity
        /// </summary>
        [JsonPropertyName("dealStock")]
        public decimal QuantityFilled { get; set; }
        /// <summary>
        /// Order quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Taker fee
        /// </summary>
        [JsonPropertyName("takerFee")]
        public decimal TakerFee { get; set; }
        /// <summary>
        /// Maker fee
        /// </summary>
        [JsonPropertyName("makerFee")]
        public decimal MakerFee { get; set; }
        /// <summary>
        /// Left
        /// </summary>
        [JsonPropertyName("left")]
        public decimal Left { get; set; }
        /// <summary>
        /// Deal fee
        /// </summary>
        [JsonPropertyName("dealFee")]
        public decimal DealFee { get; set; }
        /// <summary>
        /// Price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        /// <summary>
        /// Post only
        /// </summary>
        [JsonPropertyName("postOnly")]
        public bool PostOnly { get; set; }
        /// <summary>
        /// Immediate or cancel
        /// </summary>
        [JsonPropertyName("ioc")]
        public bool ImmediateOrCancel { get; set; }
        /// <summary>
        /// Take profit or stop loss configuration
        /// </summary>
        [JsonPropertyName("oto")]
        public WhiteBitCollateralOrderConfig? OneTriggersOtherConfig { get; set; }
    }

    /// <summary>
    /// OTO config
    /// </summary>
    public record WhiteBitCollateralOrderConfig
    {
        /// <summary>
        /// OTO id
        /// </summary>
        [JsonPropertyName("otoId")]
        public long OtoId { get; set; }
        /// <summary>
        /// Stop loss price
        /// </summary>
        [JsonPropertyName("stopLoss")]
        public decimal StopLossPrice { get; set; }
        /// <summary>
        /// Take profit price
        /// </summary>
        [JsonPropertyName("takeProfit")]
        public decimal TakeProfitPrice { get; set; }
    }


}
