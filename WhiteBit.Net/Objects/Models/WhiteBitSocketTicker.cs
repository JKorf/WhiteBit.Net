using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Ticker
    /// </summary>
    [SerializationModel]
    public record WhiteBitSocketTicker
    {
        /// <summary>
        /// ["<c>period</c>"] Period
        /// </summary>
        [JsonPropertyName("period")]
        public int Period { get; set; }
        /// <summary>
        /// ["<c>last</c>"] Last trade price
        /// </summary>
        [JsonPropertyName("last")]
        public decimal LastPrice { get; set; }
        /// <summary>
        /// ["<c>open</c>"] Open price last 24h
        /// </summary>
        [JsonPropertyName("open")]
        public decimal OpenPrice { get; set; }
        /// <summary>
        /// ["<c>close</c>"] Close price
        /// </summary>
        [JsonPropertyName("close")]
        public decimal ClosePrice { get; set; }
        /// <summary>
        /// ["<c>high</c>"] High price last 24h
        /// </summary>
        [JsonPropertyName("high")]
        public decimal HighPrice { get; set; }
        /// <summary>
        /// ["<c>low</c>"] Low price last 24h
        /// </summary>
        [JsonPropertyName("low")]
        public decimal LowPrice { get; set; }
        /// <summary>
        /// ["<c>volume</c>"] Volume
        /// </summary>
        [JsonPropertyName("volume")]
        public decimal Volume { get; set; }
        /// <summary>
        /// ["<c>deal</c>"] Volume in quote asset
        /// </summary>
        [JsonPropertyName("deal")]
        public decimal QuoteVolume { get; set; }
    }


}
