using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Order status
    /// </summary>
    [JsonConverter(typeof(EnumConverter<OrderStatus>))]
    public enum OrderStatus
    {
        /// <summary>
        /// Unfilled
        /// </summary>
        [Map("Open")]
        Open,
        /// <summary>
        /// Canceled
        /// </summary>
        [Map("Canceled", "2")]
        Canceled,
        /// <summary>
        /// Filled
        /// </summary>
        [Map("Filled", "1")]
        Filled
    }
}
