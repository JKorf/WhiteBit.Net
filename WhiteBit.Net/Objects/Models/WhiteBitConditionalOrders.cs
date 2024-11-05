using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Conditional orders
    /// </summary>
    public record WhiteBitConditionalOrdersResult
    {
        /// <summary>
        /// Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public decimal Limit { get; set; }
        /// <summary>
        /// Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public decimal Offset { get; set; }
        /// <summary>
        /// Total results
        /// </summary>
        [JsonPropertyName("total")]
        public decimal Total { get; set; }
        /// <summary>
        /// Records
        /// </summary>
        [JsonPropertyName("records")]
        public IEnumerable<WhiteBitConditionalOrders> Records { get; set; } = Array.Empty<WhiteBitConditionalOrders>();
    }

    /// <summary>
    /// Conditional order
    /// </summary>
    public record WhiteBitConditionalOrders
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        /// <summary>
        /// Order type
        /// </summary>
        [JsonPropertyName("type")]
        public ConditionalOrderType ConditionalOrderType { get; set; }
        /// <summary>
        /// Stop loss order
        /// </summary>
        [JsonPropertyName("stop_loss")]
        public WhiteBitConditionalOrder? StopLossOrder { get; set; } = null!;
        /// <summary>
        /// Take profit order
        /// </summary>
        [JsonPropertyName("take_profit")]
        public WhiteBitConditionalOrder? TakeProfitOrder { get; set; } = null!;
        /// <summary>
        /// Stop loss price
        /// </summary>
        [JsonPropertyName("stopLossPrice")]
        public decimal? StopLossPrice { get; set; }
        /// <summary>
        /// Take profit price
        /// </summary>
        [JsonPropertyName("takeProfitPrice")]
        public decimal? TakeProfitPrice { get; set; }
        /// <summary>
        /// Conditional order
        /// </summary>
        [JsonPropertyName("conditionalOrder")]
        public WhiteBitConditionalOrder? ConditionalOrder { get; set; } = null!;
    }
}
