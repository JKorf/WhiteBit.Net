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
    /// Ticker update
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<WhiteBitTickerUpdate>))]
    [SerializationModel]
    public record WhiteBitTickerUpdate
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [ArrayProperty(0)]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Trades
        /// </summary>
        [ArrayProperty(1), JsonConversion]
        public WhiteBitSocketTicker Ticker { get; set; } = null!;
    }
}
