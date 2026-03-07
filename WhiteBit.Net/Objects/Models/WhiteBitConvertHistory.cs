using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Convert history
    /// </summary>
    [SerializationModel]
    public record WhiteBitConvertHistory
    {
        /// <summary>
        /// ["<c>records</c>"] Records
        /// </summary>
        [JsonPropertyName("records")]
        public WhiteBitConvertHistoryEntry[] Records { get; set; } = Array.Empty<WhiteBitConvertHistoryEntry>();
        /// <summary>
        /// ["<c>total</c>"] Total results
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }
        /// <summary>
        /// ["<c>limit</c>"] Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
        /// <summary>
        /// ["<c>offset</c>"] Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; }
    }

    /// <summary>
    /// Convert history entry
    /// </summary>
    [SerializationModel]
    public record WhiteBitConvertHistoryEntry
    {
        /// <summary>
        /// ["<c>id</c>"] Id
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>date</c>"] Timestamp
        /// </summary>
        [JsonPropertyName("date")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// ["<c>give</c>"] From quantity
        /// </summary>
        [JsonPropertyName("give")]
        public decimal FromQuantity { get; set; }
        /// <summary>
        /// ["<c>receive</c>"] To quantity
        /// </summary>
        [JsonPropertyName("receive")]
        public decimal ToQuantity { get; set; }
        /// <summary>
        /// ["<c>rate</c>"] Conversion rate
        /// </summary>
        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }
        /// <summary>
        /// ["<c>path</c>"] Path
        /// </summary>
        [JsonPropertyName("path")]
        public WhiteBitConvertHistoryEntryPath[] Path { get; set; } = Array.Empty<WhiteBitConvertHistoryEntryPath>();
    }

    /// <summary>
    /// Convert path
    /// </summary>
    [SerializationModel]
    public record WhiteBitConvertHistoryEntryPath
    {
        /// <summary>
        /// ["<c>from</c>"] From
        /// </summary>
        [JsonPropertyName("from")]
        public string FromAsset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>to</c>"] To
        /// </summary>
        [JsonPropertyName("to")]
        public string ToAsset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>rate</c>"] Conversion rate
        /// </summary>
        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }
    }


}
