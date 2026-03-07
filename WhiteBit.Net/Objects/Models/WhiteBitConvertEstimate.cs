using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Conversion estimate
    /// </summary>
    [SerializationModel]
    public record WhiteBitConvertEstimate
    {
        /// <summary>
        /// ["<c>id</c>"] Id
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>from</c>"] From asset
        /// </summary>
        [JsonPropertyName("from")]
        public string FromAsset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>to</c>"] To asset
        /// </summary>
        [JsonPropertyName("to")]
        public string ToAsset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>give</c>"] From asset quantity
        /// </summary>
        [JsonPropertyName("give")]
        public decimal FromQuantity { get; set; }
        /// <summary>
        /// ["<c>receive</c>"] To asset quantity
        /// </summary>
        [JsonPropertyName("receive")]
        public decimal ToQuantity { get; set; }
        /// <summary>
        /// ["<c>rate</c>"] Conversion rate
        /// </summary>
        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }
        /// <summary>
        /// ["<c>expireAt</c>"] Expire timestamp
        /// </summary>
        [JsonPropertyName("expireAt")]
        public DateTime ExpireTime { get; set; }
    }


}
