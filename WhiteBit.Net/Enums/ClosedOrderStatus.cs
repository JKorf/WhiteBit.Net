using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Status of a closed order
    /// </summary>
    [JsonConverter(typeof(EnumConverter<ClosedOrderStatus>))]
    public enum ClosedOrderStatus
    {
        /// <summary>
        /// All, for filtering
        /// </summary>
        [Map("All")]
        All,
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
