using System;
using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// User trades
    /// </summary>
    [SerializationModel]
    public record WhiteBitUserTrades
    {
        /// <summary>
        /// ["<c>limit</c>"] Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
        /// <summary>
        /// ["<c>offset</c>"] Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; }
        /// <summary>
        /// ["<c>total</c>"] Total 
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }
        /// <summary>
        /// ["<c>record</c>"] Records
        /// </summary>
        [JsonPropertyName("record")]
        public WhiteBitUserTrade[] Records { get; set; } = Array.Empty<WhiteBitUserTrade>();
    }

    /// <summary>
    /// User trade
    /// </summary>
    [SerializationModel]
    public record WhiteBitUserTrade
    {
        /// <summary>
        /// Symbol
        /// </summary>
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>id</c>"] Trade id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        /// <summary>
        /// ["<c>dealOrderId</c>"] Order id
        /// </summary>
        [JsonPropertyName("dealOrderId")]
        public long OrderId { get; set; }
        [JsonInclude, JsonPropertyName("orderId")]
        internal long OrderIdInt { set { OrderId = value; } }
        [JsonInclude, JsonPropertyName("deal_order_id")]
        internal long OrderIdInt2 { set { OrderId = value; } }
        /// <summary>
        /// ["<c>clientOrderId</c>"] Client order id
        /// </summary>
        [JsonPropertyName("clientOrderId")]
        public string? ClientOrderId { get; set; }
        /// <summary>
        /// ["<c>time</c>"] Trade time
        /// </summary>
        [JsonPropertyName("time")]
        public DateTime Time { get; set; }
        /// <summary>
        /// ["<c>side</c>"] Side
        /// </summary>
        [JsonPropertyName("side")]
        public OrderSide? OrderSide { get; set; }
        /// <summary>
        /// ["<c>role</c>"] Role of the trade
        /// </summary>
        [JsonPropertyName("role")]
        public TradeRole TradeRole { get; set; }
        /// <summary>
        /// ["<c>amount</c>"] Quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>price</c>"] Price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        /// <summary>
        /// ["<c>deal</c>"] Value in quote asset
        /// </summary>
        [JsonPropertyName("deal")]
        public decimal Value { get; set; }
        /// <summary>
        /// ["<c>fee</c>"] Fee
        /// </summary>
        [JsonPropertyName("fee")]
        public decimal Fee { get; set; }

        /// <summary>
        /// ["<c>feeAsset</c>"] Fee asset
        /// </summary>
        [JsonPropertyName("feeAsset")]
        public string FeeAsset { get; set; } = string.Empty;
    }


}
