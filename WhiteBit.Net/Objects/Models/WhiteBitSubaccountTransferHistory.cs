using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Transfer history
    /// </summary>
    public record WhiteBitSubaccountTransferHistory
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
        public IEnumerable<WhiteBitSubaccountTransferEntry> Data { get; set; } = Array.Empty<WhiteBitSubaccountTransferEntry>();
    }

    /// <summary>
    /// Transfer entry
    /// </summary>
    public record WhiteBitSubaccountTransferEntry
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Transfer direction
        /// </summary>
        [JsonPropertyName("direction")]
        public SubTransferDirection Direction { get; set; }
        /// <summary>
        /// Asset
        /// </summary>
        [JsonPropertyName("currency")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// Quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Creation time
        /// </summary>
        [JsonPropertyName("createdAt")]
        public decimal CreateTime { get; set; }
    }


}
