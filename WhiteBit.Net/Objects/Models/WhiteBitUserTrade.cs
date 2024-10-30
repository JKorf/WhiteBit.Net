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
    public record WhiteBitUserTrade
    {
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
        /// <summary>
        /// Client order id
        /// </summary>
        [JsonPropertyName("clientOrderId")]
        public string ClientOrderId { get; set; } = string.Empty;
        /// <summary>
        /// Trade time
        /// </summary>
        [JsonPropertyName("time")]
        public DateTime Time { get; set; }
        /// <summary>
        /// Side
        /// </summary>
        [JsonPropertyName("side")]
        public OrderSide OrderSide { get; set; }
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
