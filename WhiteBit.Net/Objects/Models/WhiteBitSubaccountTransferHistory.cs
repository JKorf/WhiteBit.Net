using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Transfer history
    /// </summary>
    [SerializationModel]
    public record WhiteBitSubaccountTransferHistory
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
        public WhiteBitSubaccountTransferEntry[] Data { get; set; } = Array.Empty<WhiteBitSubaccountTransferEntry>();
    }

    /// <summary>
    /// Transfer entry
    /// </summary>
    [SerializationModel]
    public record WhiteBitSubaccountTransferEntry
    {
        /// <summary>
        /// ["<c>id</c>"] Id
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>direction</c>"] Transfer direction
        /// </summary>
        [JsonPropertyName("direction")]
        public SubTransferDirection Direction { get; set; }
        /// <summary>
        /// ["<c>currency</c>"] Asset
        /// </summary>
        [JsonPropertyName("currency")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>amount</c>"] Quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>createdAt</c>"] Creation time
        /// </summary>
        [JsonPropertyName("createdAt")]
        public decimal CreateTime { get; set; }
    }


}
