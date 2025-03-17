using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Ticker info
    /// </summary>
    [SerializationModel]
    public record WhiteBitTicker
    {
        /// <summary>
        /// Symbol name
        /// </summary>
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Base id
        /// </summary>
        [JsonPropertyName("base_id")]
        public long? BaseId { get; set; }
        /// <summary>
        /// Quote id
        /// </summary>
        [JsonPropertyName("quote_id")]
        public long? QuoteId { get; set; }
        /// <summary>
        /// Last trade price
        /// </summary>
        [JsonPropertyName("last_price")]
        public decimal LastPrice { get; set; }
        /// <summary>
        /// Quote volume
        /// </summary>
        [JsonPropertyName("quote_volume")]
        public decimal QuoteVolume { get; set; }
        /// <summary>
        /// Base volume
        /// </summary>
        [JsonPropertyName("base_volume")]
        public decimal BaseVolume { get; set; }
        /// <summary>
        /// Is frozen
        /// </summary>
        [JsonPropertyName("isFrozen")]
        public bool IsFrozen { get; set; }
        /// <summary>
        /// Change percentage
        /// </summary>
        [JsonPropertyName("change")]
        public decimal ChangePercentage { get; set; }
    }
}
