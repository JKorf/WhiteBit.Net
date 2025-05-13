using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Order product type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<OrderProductType>))]
    public enum OrderProductType
    {
        /// <summary>
        /// Spot
        /// </summary>
        [Map("Spot")]
        Spot,
        /// <summary>
        /// Margin
        /// </summary>
        [Map("Margin")]
        Margin,
        /// <summary>
        /// Futures
        /// </summary>
        [Map("Futures")]
        Futures
    }
}
