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
    /// Last price update
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public record WhiteBitLastPriceUpdate
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [ArrayProperty(0)]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Last price
        /// </summary>
        [ArrayProperty(1), JsonConverter(typeof(DecimalConverter))]
        public decimal LastPrice { get; set; } 
    }
}
