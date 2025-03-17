using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Liquidation status
    /// </summary>
    [JsonConverter(typeof(EnumConverter<LiquidationStatus>))]
    public enum LiquidationStatus
    {
        /// <summary>
        /// Margin call
        /// </summary>
        [Map("margin_call")]
        MarginCall,
        /// <summary>
        /// Liquidation
        /// </summary>
        [Map("liquidation")]
        Liquidation,
    }
}
