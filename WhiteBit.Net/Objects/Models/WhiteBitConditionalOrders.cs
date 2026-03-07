using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Conditional orders
    /// </summary>
    [SerializationModel]
    public record WhiteBitConditionalOrdersResult
    {
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
        /// <summary>
        /// ["<c>total</c>"] Total results
        /// </summary>
        [JsonPropertyName("total")]
        public decimal Total { get; set; }
        /// <summary>
        /// ["<c>records</c>"] Records
        /// </summary>
        [JsonPropertyName("records")]
        public WhiteBitConditionalOrders[] Records { get; set; } = Array.Empty<WhiteBitConditionalOrders>();
    }

    /// <summary>
    /// Conditional order
    /// </summary>
    [SerializationModel]
    public record WhiteBitConditionalOrders
    {
        /// <summary>
        /// ["<c>id</c>"] Id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        /// <summary>
        /// ["<c>type</c>"] Order type
        /// </summary>
        [JsonPropertyName("type")]
        public ConditionalOrderType ConditionalOrderType { get; set; }
        /// <summary>
        /// ["<c>stop_loss</c>"] Stop loss order
        /// </summary>
        [JsonPropertyName("stop_loss")]
        public WhiteBitConditionalOrder? StopLossOrder { get; set; } = null!;
        /// <summary>
        /// ["<c>take_profit</c>"] Take profit order
        /// </summary>
        [JsonPropertyName("take_profit")]
        public WhiteBitConditionalOrder? TakeProfitOrder { get; set; } = null!;
        /// <summary>
        /// ["<c>stopLossPrice</c>"] Stop loss price
        /// </summary>
        [JsonPropertyName("stopLossPrice")]
        public decimal? StopLossPrice { get; set; }
        /// <summary>
        /// ["<c>takeProfitPrice</c>"] Take profit price
        /// </summary>
        [JsonPropertyName("takeProfitPrice")]
        public decimal? TakeProfitPrice { get; set; }
        /// <summary>
        /// ["<c>conditionalOrder</c>"] Conditional order
        /// </summary>
        [JsonPropertyName("conditionalOrder")]
        public WhiteBitConditionalOrder? ConditionalOrder { get; set; } = null!;
    }
}
