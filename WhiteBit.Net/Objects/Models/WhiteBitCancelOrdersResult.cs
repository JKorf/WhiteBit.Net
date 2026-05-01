using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Cancel orders result
    /// </summary>
    public record WhiteBitCancelOrdersResult
    {
        /// <summary>
        /// Whether the order was placed successfully
        /// </summary>
        public bool Success => Error?.Code == 0;
        /// <summary>
        /// ["<c>error</c>"] Error message on fail
        /// </summary>
        [JsonPropertyName("error")]
        public WhiteBitError? Error { get; set; } = new();
        /// <summary>
        /// ["<c>result</c>"] Order result
        /// </summary>
        [JsonPropertyName("result")]
        public WhiteBitCancelResult? Result { get; set; }
    }

    /// <summary>
    /// Cancel result
    /// </summary>
    public record WhiteBitCancelResult
    {
        /// <summary>
        /// ["<c>orderId</c>"] Order id
        /// </summary>
        [JsonPropertyName("orderId")]
        public long OrderId { get; set; }
        /// <summary>
        /// ["<c>clientOrderId</c>"] Client order id
        /// </summary>
        [JsonPropertyName("clientOrderId")]
        public string? ClientOrderId { get; set; }
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
    }
}
