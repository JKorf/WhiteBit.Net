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
        /// Stop loss
        /// </summary>
        [Map("margin_stop_loss")]
        MarginStopLoss,
        /// <summary>
        /// Take profit
        /// </summary>
        [Map("margin_take_profit")]
        MarginTakeProfit
    }
}
