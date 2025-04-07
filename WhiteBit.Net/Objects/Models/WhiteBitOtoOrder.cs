using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// OTO order
    /// </summary>
    [SerializationModel]
    public record WhiteBitOtoOrder
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        /// <summary>
        /// Trigger order
        /// </summary>
        [JsonPropertyName("trigger_order")]
        public WhiteBitOrder TriggerOrder { get; set; } = null!;
        /// <summary>
        /// Stop loss price
        /// </summary>
        [JsonPropertyName("stop_loss_price")]
        public decimal? StopLossPrice { get; set; }
        /// <summary>
        /// Take profit price
        /// </summary>
        [JsonPropertyName("take_profit_price")]
        public decimal? TakeProfitPrice { get; set; }
        /// <summary>
        /// Conditional order type
        /// </summary>
        [JsonPropertyName("conditional_order_type")]
        public ConditionalType ConditionalType { get; set; }
    }
}
