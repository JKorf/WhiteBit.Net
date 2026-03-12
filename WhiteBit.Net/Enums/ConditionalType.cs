using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Conditional type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<ConditionalType>))]
    public enum ConditionalType
    {
        /// <summary>
        /// ["<c>margin_stop_loss</c>"] Stop loss
        /// </summary>
        [Map("margin_stop_loss")]
        MarginStopLoss,
        /// <summary>
        /// ["<c>margin_take_profit</c>"] Take profit
        /// </summary>
        [Map("margin_take_profit")]
        MarginTakeProfit
    }
}
