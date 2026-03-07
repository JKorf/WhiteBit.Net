using CryptoExchange.Net.Converters.SystemTextJson;
using System;
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
        /// ["<c>offset</c>"] Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public decimal Offset { get; set; }
        /// <summary>
        /// ["<c>limit</c>"] Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public decimal Limit { get; set; }
        /// <summary>
        /// ["<c>data</c>"] Data
        /// </summary>
        [JsonPropertyName("data")]
        public WhiteBitSubAccount[] Data { get; set; } = Array.Empty<WhiteBitSubAccount>();
    }

}
