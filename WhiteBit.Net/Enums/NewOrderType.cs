using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Order type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<NewOrderType>))]
    public enum NewOrderType
    {
        /// <summary>
        /// Limit
        /// </summary>
        Limit,
        /// <summary>
        /// Market
        /// </summary>
        Market,
        /// <summary>
        /// Stop limit
        /// </summary>
        StopLimit,
        /// <summary>
        /// Stop market
        /// </summary>
        StopMarket
    }
}
