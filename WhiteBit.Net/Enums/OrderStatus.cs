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
        /// ["<c>Open</c>"] Unfilled
        /// </summary>
        [Map("Open", "OPEN")]
        Open,
        /// <summary>
        /// ["<c>Canceled</c>"] Canceled
        /// </summary>
        [Map("Canceled", "CANCELED", "2")]
        Canceled,
        /// <summary>
        /// ["<c>Filled</c>"] Filled
        /// </summary>
        [Map("Filled", "FILLED", "1")]
        Filled,
        /// <summary>
        /// ["<c>PARTIALLY_FILLED</c>"] Partially filled
        /// </summary>
        [Map("PARTIALLY_FILLED")]
        PartiallyFilled,
        /// <summary>
        /// ["<c>AUTO_CANCELED_USER_MARGIN</c>"] Auto canceled due to insufficient margin
        /// </summary>
        [Map("AUTO_CANCELED_USER_MARGIN")]
        AutoCanceledUserMargin
    }
}
