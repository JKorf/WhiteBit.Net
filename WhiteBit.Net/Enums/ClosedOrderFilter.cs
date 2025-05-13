using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Closed order filter
    /// </summary>
    [JsonConverter(typeof(EnumConverter<ClosedOrderFilter>))]
    public enum ClosedOrderFilter
    {
        /// <summary>
        /// Limit and market orders
        /// </summary>
        [Map("0")]
        LimitAndMarket = 0,
        /// <summary>
        /// Only limit orders
        /// </summary>
        [Map("1")]
        Limit = 1,
        /// <summary>
        /// Only market orders
        /// </summary>
        [Map("2")]
        Market = 2
    }
}
