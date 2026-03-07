using CryptoExchange.Net.Converters.SystemTextJson;
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
        /// ["<c>id</c>"] Id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        /// <summary>
        /// ["<c>trigger_order</c>"] Trigger order
        /// </summary>
        [JsonPropertyName("trigger_order")]
        public WhiteBitOrder TriggerOrder { get; set; } = null!;
        /// <summary>
        /// ["<c>stop_loss_price</c>"] Stop loss price
        /// </summary>
        [JsonPropertyName("stop_loss_price")]
        public decimal? StopLossPrice { get; set; }
        /// <summary>
        /// ["<c>take_profit_price</c>"] Take profit price
        /// </summary>
        [JsonPropertyName("take_profit_price")]
        public decimal? TakeProfitPrice { get; set; }
        /// <summary>
        /// ["<c>conditional_order_type</c>"] Conditional order type
        /// </summary>
        [JsonPropertyName("conditional_order_type")]
        public ConditionalType ConditionalType { get; set; }
    }
}
