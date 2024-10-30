using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Bulk order request
    /// </summary>
    public record WhiteBitOrderRequest
    {
        /// <summary>
        /// Order side
        /// </summary>
        [JsonPropertyName("side")]
        public OrderSide OrderSide { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        [JsonPropertyName("amount"), JsonConverter(typeof(DecimalStringWriterConverter))]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Limit price
        /// </summary>
        [JsonPropertyName("price"), JsonConverter(typeof(DecimalStringWriterConverter))]
        public decimal Price { get; set; }
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Post only
        /// </summary>
        [JsonPropertyName("postOnly"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? PostOnly { get; set; }
        /// <summary>
        /// Immediate or cancel
        /// </summary>
        [JsonPropertyName("ioc"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? ImmediateOrCancel { get; set; }
        /// <summary>
        /// Client order id
        /// </summary>
        [JsonPropertyName("clientOrderId"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? ClientOrderId { get; set; }
    }
}
