using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Converters;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Trades update
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<WhiteBitBookUpdate>))]
    [SerializationModel]
    public record WhiteBitBookUpdate
    {
        /// <summary>
        /// Is snapshot update
        /// </summary>
        [ArrayProperty(0)]
        public bool Snapshot { get; set; }
        /// <summary>
        /// Order book
        /// </summary>
        [ArrayProperty(1), JsonConversion]
        public WhiteBitOrderBookUpdate OrderBook { get; set; } = null!;
        /// <summary>
        /// Symbol
        /// </summary>
        [ArrayProperty(2)]
        public string Symbol { get; set; } = string.Empty;
    }
}
