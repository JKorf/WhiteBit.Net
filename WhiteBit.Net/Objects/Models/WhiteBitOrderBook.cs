using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Interfaces;
using System;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Order book
    /// </summary>
    [SerializationModel]
    public record WhiteBitOrderBook
    {
        /// <summary>
        /// ["<c>ticker_id</c>"] Symbol
        /// </summary>
        [JsonPropertyName("ticker_id")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>timestamp</c>"] Timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// ["<c>asks</c>"] Asks list
        /// </summary>
        [JsonPropertyName("asks")]
        public WhiteBitOrderBookEntry[] Asks { get; set; } = Array.Empty<WhiteBitOrderBookEntry>();
        /// <summary>
        /// ["<c>bids</c>"] Bids list
        /// </summary>
        [JsonPropertyName("bids")]
        public WhiteBitOrderBookEntry[] Bids { get; set; } = Array.Empty<WhiteBitOrderBookEntry>();
    }

    /// <summary>
    /// Order book update
    /// </summary>
    public record WhiteBitOrderBookUpdate: WhiteBitOrderBook
    {
        /// <summary>
        /// ["<c>update_id</c>"] Update id
        /// </summary>
        [JsonPropertyName("update_id")]
        public long UpdateId { get; set; }
        /// <summary>
        /// ["<c>past_update_id</c>"] Previous update id
        /// </summary>
        [JsonPropertyName("past_update_id")]
        public long? PrevUpdateId { get; set; }
        /// <summary>
        /// ["<c>event_time</c>"] Event time
        /// </summary>
        [JsonPropertyName("event_time")]
        public DateTime EventTime { get; set; }
    }

    /// <summary>
    /// Order book entry
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<WhiteBitOrderBookEntry>))]
    [SerializationModel]
    public record WhiteBitOrderBookEntry : ISymbolOrderBookEntry
    {
        /// <summary>
        /// The price
        /// </summary>
        [ArrayProperty(0)]
        public decimal Price { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        [ArrayProperty(1)]
        public decimal Quantity { get; set; }
    }
}
