using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Sub account list
    /// </summary>
    [SerializationModel]
    public record WhiteBitSubAccounts
    {
        /// <summary>
        /// Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public decimal Offset { get; set; }
        /// <summary>
        /// Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public decimal Limit { get; set; }
        /// <summary>
        /// Data
        /// </summary>
        [JsonPropertyName("data")]
        public WhiteBitSubAccount[] Data { get; set; } = Array.Empty<WhiteBitSubAccount>();
    }

}
