using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Order info
    /// </summary>
    public record WhiteBitOrder
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
        /// Symbol name
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
        /// Create timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime CreateTime { get; set; }

        [JsonInclude, JsonPropertyName("ctime")]
        internal DateTime CreateTimeInt { set { CreateTime = value; } }

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
        /// Remaining quantity to be filled
        /// </summary>
        [JsonPropertyName("left")]
        public decimal QuantityRemaining { get; set; }
        /// <summary>
        /// Order fee
        /// </summary>
        [JsonPropertyName("dealFee")]
        public decimal Fee { get; set; }
        /// <summary>
        /// Limit price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
        /// <summary>
        /// Post only
        /// </summary>
        [JsonPropertyName("postOnly")]
        public bool PostOnly { get; set; }
        /// <summary>
        /// Immediate or cancel flag
        /// </summary>
        [JsonPropertyName("ioc")]
        public bool ImmediateOrCancel { get; set; }
        /// <summary>
        /// Trigger price
        /// </summary>
        [JsonPropertyName("activation_price")]
        public decimal? TriggerPrice { get; set; }
    }

    /// <summary>
    /// Closed order info
    /// </summary>
    public record WhiteBitClosedOrder : WhiteBitOrder
    {
        /// <summary>
        /// Filled timestamp
        /// </summary>
        [JsonPropertyName("ftime")]
        public DateTime FillTime { get; set; }

        /// <summary>
        /// Closed order status
        /// </summary>
        [JsonPropertyName("status")]
        public ClosedOrderStatus Status { get; set; }
    }
}
