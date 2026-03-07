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
    public record WhiteBitTrade
    {
        /// <summary>
        /// ["<c>tradeID</c>"] Trade id
        /// </summary>
        [JsonPropertyName("tradeID")]
        public long TradeId { get; set; }
        /// <summary>
        /// ["<c>price</c>"] Price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        /// <summary>
        /// ["<c>base_volume</c>"] Quote volume
        /// </summary>
        [JsonPropertyName("base_volume")]
        // THE VOLUME PROPERTIES ARE INCORRECTLY INVERSED IN THE API RESPONSE
        public decimal QuoteVolume { get; set; }
        /// <summary>
        /// Base volume
        /// </summary>
        // THE VOLUME PROPERTIES ARE INCORRECTLY INVERSED IN THE API RESPONSE
        [JsonPropertyName("quote_volume")]
        public decimal BaseVolume { get; set; }
        /// <summary>
        /// ["<c>trade_timestamp</c>"] Trade timestamp
        /// </summary>
        [JsonPropertyName("trade_timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// ["<c>type</c>"] Trade side
        /// </summary>
        [JsonPropertyName("type")]
        public OrderSide Side { get; set; }
    }


}
