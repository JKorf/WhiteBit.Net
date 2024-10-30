using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Conversion estimate
    /// </summary>
    public record WhiteBitConvertEstimate
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// From asset
        /// </summary>
        [JsonPropertyName("from")]
        public string FromAsset { get; set; } = string.Empty;
        /// <summary>
        /// To asset
        /// </summary>
        [JsonPropertyName("to")]
        public string ToAsset { get; set; } = string.Empty;
        /// <summary>
        /// From asset quantity
        /// </summary>
        [JsonPropertyName("give")]
        public decimal FromQuantity { get; set; }
        /// <summary>
        /// To asset quantity
        /// </summary>
        [JsonPropertyName("receive")]
        public decimal ToQuantity { get; set; }
        /// <summary>
        /// Conversion rate
        /// </summary>
        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }
        /// <summary>
        /// Expire timestamp
        /// </summary>
        [JsonPropertyName("expireAt")]
        public DateTime ExpireTime { get; set; }
    }


}
