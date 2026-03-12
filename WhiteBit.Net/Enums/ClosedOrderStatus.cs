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
        /// ["<c>All</c>"] All, for filtering
        /// </summary>
        [Map("All")]
        All,
        /// <summary>
        /// ["<c>Canceled</c>"] Canceled
        /// </summary>
        [Map("Canceled", "2")]
        Canceled,
        /// <summary>
        /// ["<c>Filled</c>"] Filled
        /// </summary>
        [Map("Filled", "1")]
        Filled
    }
}
