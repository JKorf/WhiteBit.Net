using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Bulk order response
    /// </summary>
    public record WhiteBitOrderResponse
    {
        /// <summary>
        /// Whether the order was placed successfully
        /// </summary>
        public bool Success => Error?.Code == 0;
        /// <summary>
        /// Error message on fail
        /// </summary>
        [JsonPropertyName("error")]
        public WhiteBitError Error { get; set; } = new();
        /// <summary>
        /// Order result
        /// </summary>
        [JsonPropertyName("result")]
        public WhiteBitOrder? Result { get; set; }
    }
}
