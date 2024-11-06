using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// User trades
    /// </summary>
    public record WhiteBitUserTrades
    {
        /// <summary>
        /// Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
        /// <summary>
        /// Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; }
        /// <summary>
        /// Total 
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }
        /// <summary>
        /// Records
        /// </summary>
        [JsonPropertyName("record")]
        public IEnumerable<WhiteBitUserTrade> Records { get; set; } = Array.Empty<WhiteBitUserTrade>();
    }

    /// <summary>
    /// User trade
    /// </summary>
    public record WhiteBitUserTrade
    {
        /// <summary>
        /// Symbol
        /// </summary>
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Trade id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        /// <summary>
        /// Order id
        /// </summary>
        [JsonPropertyName("dealOrderId")]
        public long OrderId { get; set; }
        [JsonInclude, JsonPropertyName("orderId")]
        internal long OrderIdInt { set { OrderId = value; } }
        [JsonInclude, JsonPropertyName("deal_order_id")]
        internal long OrderIdInt2 { set { OrderId = value; } }
        /// <summary>
        /// Client order id
        /// </summary>
        [JsonPropertyName("clientOrderId")]
        public string? ClientOrderId { get; set; }
        /// <summary>
        /// Trade time
        /// </summary>
        [JsonPropertyName("time")]
        public DateTime Time { get; set; }
        /// <summary>
        /// Side
        /// </summary>
        [JsonPropertyName("side")]
        public OrderSide? OrderSide { get; set; }
        /// <summary>
        /// Role of the trade
        /// </summary>
        [JsonPropertyName("role")]
        public TradeRole TradeRole { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        /// <summary>
        /// Value in quote asset
        /// </summary>
        [JsonPropertyName("deal")]
        public decimal Value { get; set; }
        /// <summary>
        /// Fee
        /// </summary>
        [JsonPropertyName("fee")]
        public decimal Fee { get; set; }
    }


}
