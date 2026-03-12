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
        /// ["<c>Spot</c>"] Spot
        /// </summary>
        [Map("Spot")]
        Spot,
        /// <summary>
        /// ["<c>Margin</c>"] Margin
        /// </summary>
        [Map("Margin")]
        Margin,
        /// <summary>
        /// ["<c>Futures</c>"] Futures
        /// </summary>
        [Map("Futures")]
        Futures
    }
}
