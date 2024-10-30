using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Ticker
    /// </summary>
    public record WhiteBitSocketTicker
    {
        /// <summary>
        /// Period
        /// </summary>
        [JsonPropertyName("period")]
        public int Period { get; set; }
        /// <summary>
        /// Last trade price
        /// </summary>
        [JsonPropertyName("last")]
        public decimal LastPrice { get; set; }
        /// <summary>
        /// Open price last 24h
        /// </summary>
        [JsonPropertyName("open")]
        public decimal OpenPrice { get; set; }
        /// <summary>
        /// Close price
        /// </summary>
        [JsonPropertyName("close")]
        public decimal ClosePrice { get; set; }
        /// <summary>
        /// High price last 24h
        /// </summary>
        [JsonPropertyName("high")]
        public decimal HighPrice { get; set; }
        /// <summary>
        /// Low price last 24h
        /// </summary>
        [JsonPropertyName("low")]
        public decimal LowPrice { get; set; }
        /// <summary>
        /// Volume
        /// </summary>
        [JsonPropertyName("volume")]
        public decimal Volume { get; set; }
        /// <summary>
        /// Volume in quote asset
        /// </summary>
        [JsonPropertyName("deal")]
        public decimal QuoteVolume { get; set; }
    }


}
