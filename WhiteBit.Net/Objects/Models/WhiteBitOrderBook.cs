using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Converters;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Order book
    /// </summary>
    [SerializationModel]
    public record WhiteBitOrderBook
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("ticker_id")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Asks list
        /// </summary>
        [JsonPropertyName("asks")]
        public WhiteBitOrderBookEntry[] Asks { get; set; } = Array.Empty<WhiteBitOrderBookEntry>();
        /// <summary>
        /// Bids list
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
        /// Update id
        /// </summary>
        [JsonPropertyName("update_id")]
        public long UpdateId { get; set; }
        /// <summary>
        /// Previous update id
        /// </summary>
        [JsonPropertyName("past_update_id")]
        public long? PrevUpdateId { get; set; }
        /// <summary>
        /// Event time
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
