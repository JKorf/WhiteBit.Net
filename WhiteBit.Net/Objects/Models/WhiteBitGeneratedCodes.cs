using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Generated codes history
    /// </summary>
    [SerializationModel]
    public record WhiteBitGeneratedCodes
    {
        /// <summary>
        /// ["<c>total</c>"] Total results
        /// </summary>
        [JsonPropertyName("total")]
        public decimal Total { get; set; }
        /// <summary>
        /// ["<c>data</c>"] Data
        /// </summary>
        [JsonPropertyName("data")]
        public WhiteBitGeneratedCode[] Data { get; set; } = Array.Empty<WhiteBitGeneratedCode>();
        /// <summary>
        /// ["<c>limit</c>"] Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public decimal Limit { get; set; }
        /// <summary>
        /// ["<c>offset</c>"] Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public decimal Offset { get; set; }
    }

    /// <summary>
    /// Generated code
    /// </summary>
    [SerializationModel]
    public record WhiteBitGeneratedCode
    {
        /// <summary>
        /// ["<c>amount</c>"] Quantity, positive means code is applied, negative means code was created
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>code</c>"] Code
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>date</c>"] Creation time
        /// </summary>
        [JsonPropertyName("date")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// ["<c>status</c>"] Status
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>ticker</c>"] Asset name
        /// </summary>
        [JsonPropertyName("ticker")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>external_id</c>"] External id
        /// </summary>
        [JsonPropertyName("external_id")]
        public string ExternalId { get; set; } = string.Empty;
    }


}
