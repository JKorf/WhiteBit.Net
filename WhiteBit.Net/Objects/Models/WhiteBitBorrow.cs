using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Borrows result
    /// </summary>
    [SerializationModel]
    public record WhiteBitBorrows
    {
        /// <summary>
        /// ["<c>total</c>"] Total
        /// </summary>
        [JsonPropertyName("total")]
        public decimal Total { get; set; }
        /// <summary>
        /// ["<c>records</c>"] Records
        /// </summary>
        [JsonPropertyName("records")]
        public WhiteBitBorrow[] Records { get; set; } = Array.Empty<WhiteBitBorrow>();
    }

    /// <summary>
    /// Borrow info
    /// </summary>
    [SerializationModel]
    public record WhiteBitBorrow
    {
        /// <summary>
        /// ["<c>asset</c>"] Asset
        /// </summary>
        [JsonPropertyName("asset")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>ctime</c>"] Create time
        /// </summary>
        [JsonPropertyName("ctime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// ["<c>mtime</c>"] Update time
        /// </summary>
        [JsonPropertyName("mtime")]
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// ["<c>amount</c>"] Quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>unrealized_funding</c>"] Unrealized funding
        /// </summary>
        [JsonPropertyName("unrealized_funding")]
        public decimal UnrealizedFunding { get; set; }
        /// <summary>
        /// ["<c>liq_price</c>"] Liquidation price
        /// </summary>
        [JsonPropertyName("liq_price")]
        public decimal LiquidationPrice { get; set; }
    }


}
