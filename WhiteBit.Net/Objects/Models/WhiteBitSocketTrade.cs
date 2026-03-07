using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Trade info
    /// </summary>
    [SerializationModel]
    public record WhiteBitSocketTrade
    {
        /// <summary>
        /// ["<c>id</c>"] Trade id
        /// </summary>
        [JsonPropertyName("id")]
        public long TradeId { get; set; }
        /// <summary>
        /// ["<c>price</c>"] Price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        /// <summary>
        /// ["<c>amount</c>"] Base volume
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>time</c>"] Trade timestamp
        /// </summary>
        [JsonPropertyName("time")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// ["<c>type</c>"] Trade side
        /// </summary>
        [JsonPropertyName("type")]
        public OrderSide Side { get; set; }
    }


}
