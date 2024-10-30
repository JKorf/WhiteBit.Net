using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Convert history
    /// </summary>
    public record WhiteBitConvertHistory
    {
        /// <summary>
        /// Records
        /// </summary>
        [JsonPropertyName("records")]
        public IEnumerable<WhiteBitConvertHistoryEntry> Records { get; set; } = Array.Empty<WhiteBitConvertHistoryEntry>();
        /// <summary>
        /// Total results
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }
        /// <summary>
        /// Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
        /// <summary>
        /// Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; }
    }

    /// <summary>
    /// Convert history entry
    /// </summary>
    public record WhiteBitConvertHistoryEntry
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("date")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// From quantity
        /// </summary>
        [JsonPropertyName("give")]
        public decimal FromQuantity { get; set; }
        /// <summary>
        /// To quantity
        /// </summary>
        [JsonPropertyName("receive")]
        public decimal ToQuantity { get; set; }
        /// <summary>
        /// Conversion rate
        /// </summary>
        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }
        /// <summary>
        /// Path
        /// </summary>
        [JsonPropertyName("path")]
        public IEnumerable<WhiteBitConvertHistoryEntryPath> Path { get; set; } = Array.Empty<WhiteBitConvertHistoryEntryPath>();
    }

    /// <summary>
    /// Convert path
    /// </summary>
    public record WhiteBitConvertHistoryEntryPath
    {
        /// <summary>
        /// From
        /// </summary>
        [JsonPropertyName("from")]
        public string FromAsset { get; set; } = string.Empty;
        /// <summary>
        /// To
        /// </summary>
        [JsonPropertyName("to")]
        public string ToAsset { get; set; } = string.Empty;
        /// <summary>
        /// Conversion rate
        /// </summary>
        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }
    }


}
