using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// TP/SL orders
    /// </summary>
    [SerializationModel]
    public record WhiteBitTpSl
    {
        /// <summary>
        /// ["<c>stop_loss</c>"] Stop loss order
        /// </summary>
        [JsonPropertyName("stop_loss")]
        public WhiteBitConditionalOrder StopLossOrder { get; set; } = null!;
        /// <summary>
        /// ["<c>take_profit</c>"] Take profit order
        /// </summary>
        [JsonPropertyName("take_profit")]
        public WhiteBitConditionalOrder TakeProfitOrder { get; set; } = null!;
    }
}
