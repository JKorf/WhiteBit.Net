using CryptoExchange.Net.Converters.SystemTextJson;
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
        /// ["<c>base_id</c>"] Base id
        /// </summary>
        [JsonPropertyName("base_id")]
        public long? BaseId { get; set; }
        /// <summary>
        /// ["<c>quote_id</c>"] Quote id
        /// </summary>
        [JsonPropertyName("quote_id")]
        public long? QuoteId { get; set; }
        /// <summary>
        /// ["<c>last_price</c>"] Last trade price
        /// </summary>
        [JsonPropertyName("last_price")]
        public decimal LastPrice { get; set; }
        /// <summary>
        /// ["<c>quote_volume</c>"] Quote volume
        /// </summary>
        [JsonPropertyName("quote_volume")]
        public decimal QuoteVolume { get; set; }
        /// <summary>
        /// ["<c>base_volume</c>"] Base volume
        /// </summary>
        [JsonPropertyName("base_volume")]
        public decimal BaseVolume { get; set; }
        /// <summary>
        /// ["<c>isFrozen</c>"] Is frozen
        /// </summary>
        [JsonPropertyName("isFrozen")]
        public bool IsFrozen { get; set; }
        /// <summary>
        /// ["<c>change</c>"] Change percentage
        /// </summary>
        [JsonPropertyName("change")]
        public decimal ChangePercentage { get; set; }
    }
}
