using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Trades update
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public record WhiteBitBookUpdate
    {
        /// <summary>
        /// Is snapshot update
        /// </summary>
        [ArrayProperty(0)]
        public bool Snapshot { get; set; }
        /// <summary>
        /// Trades
        /// </summary>
        [ArrayProperty(1), JsonConversion]
        public WhiteBitOrderBook OrderBook { get; set; } = null!;
        /// <summary>
        /// Symbol
        /// </summary>
        [ArrayProperty(2)]
        public string Symbol { get; set; } = string.Empty;
    }
}
