using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Trading fee
    /// </summary>
    [SerializationModel]
    public record WhiteBitTradingFee
    {
        /// <summary>
        /// Taker fee rate
        /// </summary>
        [JsonPropertyName("taker")]
        public decimal TakerFee { get; set; }
        /// <summary>
        /// Maker fee rate
        /// </summary>
        [JsonPropertyName("maker")]
        public decimal MakerFee { get; set; }
    }
}
